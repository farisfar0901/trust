using Trust.Api.Application.Services.Authentication.DTOs;

namespace Trust.Api.Application.Services.Authentication;

/// <summary>
/// Business logic for admin login, token refresh, and logout. This service
/// does not perform any ASP.NET Core authentication/authorization pipeline
/// work — it only decides whether credentials and tokens are valid, and
/// issues or revokes tokens accordingly.
/// </summary>
public interface IAdminAuthService
{
    /// <summary>
    /// Verifies an admin's credentials and, if valid, issues a new access
    /// token and refresh token.
    /// </summary>
    /// <param name="request">The submitted email and password.</param>
    /// <param name="ipAddress">The caller's IP address, recorded on the issued refresh token.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A successful result with tokens, or a failed result explaining why.</returns>
    Task<LoginResult> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken cancellationToken);

    /// <summary>
    /// Exchanges a still-valid refresh token for a new access token and a
    /// new refresh token, revoking the old refresh token in the process.
    /// </summary>
    /// <param name="request">The refresh token to exchange.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A successful result with new tokens, or a failed result explaining why.</returns>
    Task<RefreshTokenResult> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Revokes a refresh token so it can no longer be used to obtain new
    /// access tokens, ending the admin's session.
    /// </summary>
    /// <param name="request">The refresh token to revoke.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken);
}
