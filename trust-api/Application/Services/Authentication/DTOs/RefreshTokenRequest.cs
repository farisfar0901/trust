using System.ComponentModel.DataAnnotations;

namespace Trust.Api.Application.Services.Authentication.DTOs;

/// <summary>
/// A request to exchange a refresh token for a new access token.
/// </summary>
public sealed class RefreshTokenRequest
{
    /// <summary>
    /// The raw refresh token previously issued at login.
    /// </summary>
    [Required]
    public required string RefreshToken { get; init; }
}
