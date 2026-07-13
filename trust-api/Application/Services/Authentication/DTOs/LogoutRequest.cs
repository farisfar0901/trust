using System.ComponentModel.DataAnnotations;

namespace Trust.Api.Application.Services.Authentication.DTOs;

/// <summary>
/// A request to invalidate a refresh token, ending an admin's session.
/// </summary>
public sealed class LogoutRequest
{
    /// <summary>
    /// The raw refresh token to revoke.
    /// </summary>
    [Required]
    public required string RefreshToken { get; init; }
}
