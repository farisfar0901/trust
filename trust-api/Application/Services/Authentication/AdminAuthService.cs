using System.Net;
using Trust.Api.Application.Common;
using Trust.Api.Application.Repositories;
using Trust.Api.Application.Services.Authentication.DTOs;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Application.Services.Authentication;

/// <summary>
/// Default <see cref="IAdminAuthService"/> implementation. Handles credential
/// verification, brute-force lockout tracking, and issuing/rotating/revoking
/// refresh tokens.
/// </summary>
public sealed class AdminAuthService : IAdminAuthService
{
    /// <summary>
    /// Number of consecutive failed login attempts allowed before an
    /// account is temporarily locked.
    /// </summary>
    private const int MaxFailedLoginAttempts = 5;

    /// <summary>
    /// How long an account stays locked after too many failed attempts.
    /// </summary>
    private const int LockoutDurationMinutes = 15;

    /// <summary>
    /// How long a refresh token remains valid after being issued.
    /// </summary>
    private const int RefreshTokenLifetimeDays = 7;

    private const string RefreshTokenType = "RefreshToken";

    private readonly IAdminUserRepository _adminUserRepository;
    private readonly IAdminAuthTokenRepository _authTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly ISecureTokenGenerator _secureTokenGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>
    /// Creates the service.
    /// </summary>
    public AdminAuthService(
        IAdminUserRepository adminUserRepository,
        IAdminAuthTokenRepository authTokenRepository,
        IPasswordHasher passwordHasher,
        IAccessTokenGenerator accessTokenGenerator,
        ISecureTokenGenerator secureTokenGenerator,
        IDateTimeProvider dateTimeProvider)
    {
        _adminUserRepository = adminUserRepository;
        _authTokenRepository = authTokenRepository;
        _passwordHasher = passwordHasher;
        _accessTokenGenerator = accessTokenGenerator;
        _secureTokenGenerator = secureTokenGenerator;
        _dateTimeProvider = dateTimeProvider;
    }

    /// <inheritdoc />
    public async Task<LoginResult> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var admin = await _adminUserRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (admin is null || admin.DeletedAt is not null)
        {
            // Same failure reason as a wrong password, so callers cannot use
            // the response to discover which email addresses have accounts.
            return LoginResult.Failure(LoginFailureReason.InvalidCredentials);
        }

        if (!admin.IsActive)
        {
            return LoginResult.Failure(LoginFailureReason.AccountInactive);
        }

        if (admin.LockedUntil.HasValue && admin.LockedUntil.Value > _dateTimeProvider.UtcNow)
        {
            return LoginResult.Failure(LoginFailureReason.AccountLocked);
        }

        if (!_passwordHasher.VerifyPassword(request.Password, admin.PasswordHash))
        {
            await RecordFailedLoginAttemptAsync(admin, cancellationToken);
            return LoginResult.Failure(LoginFailureReason.InvalidCredentials);
        }

        await RecordSuccessfulLoginAsync(admin, ipAddress, cancellationToken);

        var (accessToken, refreshToken, refreshTokenExpiresAt) =
            await IssueTokenPairAsync(admin, ipAddress, cancellationToken);

        return LoginResult.Success(new LoginResponse
        {
            AccessToken = accessToken.Token,
            AccessTokenExpiresAt = accessToken.ExpiresAt,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,
            AdminId = admin.Id,
            FullName = admin.FullName,
            Email = admin.Email,
            Role = admin.Role,
        });
    }

    /// <inheritdoc />
    public async Task<RefreshTokenResult> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tokenHash = _secureTokenGenerator.Hash(request.RefreshToken);
        var existingToken = await _authTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        var validationFailure = ValidateRefreshToken(existingToken);
        if (validationFailure is not null)
        {
            return RefreshTokenResult.Failure(validationFailure.Value);
        }

        var admin = await _adminUserRepository.GetByIdAsync(existingToken!.AdminId, cancellationToken);
        if (admin is null || admin.DeletedAt is not null || !admin.IsActive)
        {
            return RefreshTokenResult.Failure(RefreshTokenFailureReason.AccountInactive);
        }

        // Rotate: the old refresh token is marked used and cannot be
        // exchanged again, even if it has not yet expired.
        existingToken.UsedAt = _dateTimeProvider.UtcNow;
        await _authTokenRepository.UpdateAsync(existingToken, cancellationToken);

        var (accessToken, refreshToken, refreshTokenExpiresAt) =
            await IssueTokenPairAsync(admin, existingToken.CreatedIp?.ToString(), cancellationToken);

        return RefreshTokenResult.Success(new RefreshTokenResponse
        {
            AccessToken = accessToken.Token,
            AccessTokenExpiresAt = accessToken.ExpiresAt,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,
        });
    }

    /// <inheritdoc />
    public async Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tokenHash = _secureTokenGenerator.Hash(request.RefreshToken);
        var existingToken = await _authTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        // Logging out an already-invalid or unknown token is treated as a
        // no-op, not an error — the caller's goal (not being logged in) is
        // already satisfied.
        if (existingToken is null || existingToken.RevokedAt is not null)
        {
            return;
        }

        existingToken.RevokedAt = _dateTimeProvider.UtcNow;
        await _authTokenRepository.UpdateAsync(existingToken, cancellationToken);
    }

    /// <summary>
    /// Checks whether a refresh token can be exchanged for a new access
    /// token, without yet checking the owning admin's status.
    /// </summary>
    /// <returns>The failure reason, or <see langword="null"/> if the token is valid.</returns>
    private RefreshTokenFailureReason? ValidateRefreshToken(AdminAuthToken? token)
    {
        if (token is null || token.TokenType != RefreshTokenType || token.RevokedAt is not null || token.UsedAt is not null)
        {
            return RefreshTokenFailureReason.InvalidToken;
        }

        if (token.ExpiresAt < _dateTimeProvider.UtcNow)
        {
            return RefreshTokenFailureReason.TokenExpired;
        }

        return null;
    }

    /// <summary>
    /// Increments the failed login counter and locks the account once the
    /// threshold is reached.
    /// </summary>
    private async Task RecordFailedLoginAttemptAsync(AdminUser admin, CancellationToken cancellationToken)
    {
        admin.FailedLoginAttempts++;

        if (admin.FailedLoginAttempts >= MaxFailedLoginAttempts)
        {
            admin.LockedUntil = _dateTimeProvider.UtcNow.AddMinutes(LockoutDurationMinutes);
        }

        await _adminUserRepository.UpdateAsync(admin, cancellationToken);
    }

    /// <summary>
    /// Resets the failed login counter and records when and from where the
    /// admin last logged in successfully.
    /// </summary>
    private async Task RecordSuccessfulLoginAsync(AdminUser admin, string? ipAddress, CancellationToken cancellationToken)
    {
        admin.FailedLoginAttempts = 0;
        admin.LockedUntil = null;
        admin.LastLoginAt = _dateTimeProvider.UtcNow;
        admin.LastLoginIp = ParseIpAddress(ipAddress);

        await _adminUserRepository.UpdateAsync(admin, cancellationToken);
    }

    /// <summary>
    /// Issues a new access token and a new, persisted refresh token for the
    /// given admin. Shared by login and token refresh so both flows use the
    /// exact same token-issuing logic.
    /// </summary>
    private async Task<(AccessToken AccessToken, string RefreshToken, DateTimeOffset RefreshTokenExpiresAt)> IssueTokenPairAsync(
        AdminUser admin, string? ipAddress, CancellationToken cancellationToken)
    {
        var accessToken = _accessTokenGenerator.GenerateAccessToken(admin);

        var rawRefreshToken = _secureTokenGenerator.GenerateToken();
        var refreshTokenExpiresAt = _dateTimeProvider.UtcNow.AddDays(RefreshTokenLifetimeDays);

        var refreshTokenEntity = new AdminAuthToken
        {
            AdminId = admin.Id,
            TokenType = RefreshTokenType,
            TokenHash = _secureTokenGenerator.Hash(rawRefreshToken),
            ExpiresAt = refreshTokenExpiresAt,
            CreatedIp = ParseIpAddress(ipAddress),
        };
        await _authTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);

        return (accessToken, rawRefreshToken, refreshTokenExpiresAt);
    }

    /// <summary>
    /// Parses an IP address string, returning <see langword="null"/> instead
    /// of throwing if it is missing or not a valid address.
    /// </summary>
    private static IPAddress? ParseIpAddress(string? ipAddress)
        => IPAddress.TryParse(ipAddress, out var parsed) ? parsed : null;
}
