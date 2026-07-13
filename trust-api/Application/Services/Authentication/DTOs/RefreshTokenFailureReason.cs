namespace Trust.Api.Application.Services.Authentication.DTOs;

/// <summary>
/// The reason a token refresh attempt did not succeed.
/// </summary>
public enum RefreshTokenFailureReason
{
    /// <summary>
    /// The token does not exist, is not a refresh token, has already been
    /// used, or has been revoked.
    /// </summary>
    InvalidToken,

    /// <summary>
    /// The token exists but its expiry date has passed.
    /// </summary>
    TokenExpired,

    /// <summary>
    /// The token is valid but the admin it belongs to is no longer active.
    /// </summary>
    AccountInactive,
}
