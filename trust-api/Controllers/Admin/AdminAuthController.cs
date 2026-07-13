using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trust.Api.Application.Services.Authentication;
using Trust.Api.Application.Services.Authentication.DTOs;
using Trust.Api.Authorization;

namespace Trust.Api.Controllers.Admin;

/// <summary>
/// HTTP endpoints for admin login, token refresh, and logout. This
/// controller only translates HTTP requests into calls on
/// <see cref="IAdminAuthService"/> and maps the results to HTTP responses —
/// all the actual authentication logic lives in the service layer.
/// </summary>
[ApiController]
[Route("api/admin/auth")]
public sealed class AdminAuthController : ControllerBase
{
    private readonly IAdminAuthService _adminAuthService;

    /// <summary>
    /// Creates the controller.
    /// </summary>
    /// <param name="adminAuthService">The service that performs credential checks and issues tokens.</param>
    public AdminAuthController(IAdminAuthService adminAuthService)
    {
        _adminAuthService = adminAuthService;
    }

    /// <summary>
    /// Logs an admin in and issues a new access token and refresh token.
    /// </summary>
    /// <param name="request">The admin's email and password.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status423Locked)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _adminAuthService.LoginAsync(request, ipAddress, cancellationToken);

        return result.IsSuccess ? Ok(result.Response) : MapLoginFailure(result.FailureReason!.Value);
    }

    /// <summary>
    /// Exchanges a still-valid refresh token for a new access token and a
    /// new refresh token. The refresh token used in the request is revoked
    /// as part of this call and cannot be reused.
    /// </summary>
    /// <param name="request">The refresh token to exchange.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _adminAuthService.RefreshTokenAsync(request, cancellationToken);

        return result.IsSuccess ? Ok(result.Response) : MapRefreshFailure(result.FailureReason!.Value);
    }

    /// <summary>
    /// Revokes a refresh token, ending the admin's session. This endpoint
    /// does not require a currently-valid access token — only the refresh
    /// token itself — so an admin can still log out after their access
    /// token has expired.
    /// </summary>
    /// <param name="request">The refresh token to revoke.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpPost("logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken cancellationToken)
    {
        await _adminAuthService.LogoutAsync(request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Returns the identity of the currently authenticated admin. Requires a
    /// valid access token and is used to prove the authorization pipeline
    /// (JWT validation plus role checking) is working end to end.
    /// </summary>
    [HttpGet("me")]
    [RequireAdminRole("SuperAdmin", "Admin", "Editor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetCurrentAdmin()
    {
        return Ok(new
        {
            AdminId = User.GetAdminId(),
            Email = User.GetAdminEmail(),
            Role = User.GetAdminRole(),
        });
    }

    /// <summary>
    /// Maps a login failure reason to the appropriate HTTP problem response.
    /// </summary>
    private ObjectResult MapLoginFailure(LoginFailureReason reason) => reason switch
    {
        LoginFailureReason.InvalidCredentials => Problem(
            title: "Invalid credentials",
            detail: "The email or password is incorrect.",
            statusCode: StatusCodes.Status401Unauthorized),
        LoginFailureReason.AccountLocked => Problem(
            title: "Account locked",
            detail: "This account is temporarily locked after too many failed login attempts.",
            statusCode: StatusCodes.Status423Locked),
        LoginFailureReason.AccountInactive => Problem(
            title: "Account inactive",
            detail: "This account has been deactivated.",
            statusCode: StatusCodes.Status403Forbidden),
        _ => Problem(detail: "Login failed.", statusCode: StatusCodes.Status401Unauthorized),
    };

    /// <summary>
    /// Maps a refresh-token failure reason to the appropriate HTTP problem response.
    /// </summary>
    private ObjectResult MapRefreshFailure(RefreshTokenFailureReason reason) => reason switch
    {
        RefreshTokenFailureReason.InvalidToken => Problem(
            title: "Invalid refresh token",
            detail: "The refresh token is invalid, revoked, or has already been used.",
            statusCode: StatusCodes.Status401Unauthorized),
        RefreshTokenFailureReason.TokenExpired => Problem(
            title: "Refresh token expired",
            detail: "The refresh token has expired. Please log in again.",
            statusCode: StatusCodes.Status401Unauthorized),
        RefreshTokenFailureReason.AccountInactive => Problem(
            title: "Account inactive",
            detail: "This account has been deactivated.",
            statusCode: StatusCodes.Status403Forbidden),
        _ => Problem(detail: "Token refresh failed.", statusCode: StatusCodes.Status401Unauthorized),
    };
}
