namespace Trust.Api.Application.Services.Authentication.DTOs;

/// <summary>
/// The reason a login attempt did not succeed.
/// </summary>
public enum LoginFailureReason
{
    /// <summary>
    /// The email was not found, or the password did not match. The two
    /// cases are deliberately not distinguished so a caller cannot use the
    /// error to discover which email addresses have accounts.
    /// </summary>
    InvalidCredentials,

    /// <summary>
    /// The account exists but has been temporarily locked after too many
    /// failed login attempts.
    /// </summary>
    AccountLocked,

    /// <summary>
    /// The account exists but has been deactivated by another admin.
    /// </summary>
    AccountInactive,
}
