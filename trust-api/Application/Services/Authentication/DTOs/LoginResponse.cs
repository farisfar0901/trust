namespace Trust.Api.Application.Services.Authentication.DTOs;

/// <summary>
/// Tokens and profile summary returned after a successful login.
/// </summary>
public sealed class LoginResponse
{
    /// <summary>
    /// The short-lived signed access token to send as a bearer token on
    /// subsequent API requests.
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    /// The moment <see cref="AccessToken"/> stops being valid.
    /// </summary>
    public required DateTimeOffset AccessTokenExpiresAt { get; init; }

    /// <summary>
    /// The long-lived opaque token used to obtain a new access token once
    /// this one expires. Store it securely — it is only shown once.
    /// </summary>
    public required string RefreshToken { get; init; }

    /// <summary>
    /// The moment <see cref="RefreshToken"/> stops being valid.
    /// </summary>
    public required DateTimeOffset RefreshTokenExpiresAt { get; init; }

    /// <summary>
    /// The id of the admin who logged in.
    /// </summary>
    public required long AdminId { get; init; }

    /// <summary>
    /// The admin's display name.
    /// </summary>
    public required string FullName { get; init; }

    /// <summary>
    /// The admin's email address.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// The admin's role (for example <c>Admin</c> or <c>Editor</c>).
    /// </summary>
    public required string Role { get; init; }
}
