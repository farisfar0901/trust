namespace Trust.Api.Application.Services.Authentication.DTOs;

/// <summary>
/// A freshly issued pair of tokens, returned after a successful token refresh.
/// </summary>
public sealed class RefreshTokenResponse
{
    /// <summary>
    /// The new short-lived signed access token.
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    /// The moment <see cref="AccessToken"/> stops being valid.
    /// </summary>
    public required DateTimeOffset AccessTokenExpiresAt { get; init; }

    /// <summary>
    /// The new long-lived opaque refresh token. The previous refresh token
    /// is no longer usable once this one is issued.
    /// </summary>
    public required string RefreshToken { get; init; }

    /// <summary>
    /// The moment <see cref="RefreshToken"/> stops being valid.
    /// </summary>
    public required DateTimeOffset RefreshTokenExpiresAt { get; init; }
}
