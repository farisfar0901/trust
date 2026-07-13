using System.ComponentModel.DataAnnotations;

namespace Trust.Api.Application.Services.Authentication.DTOs;

/// <summary>
/// Credentials submitted by an admin trying to log in.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>
    /// The admin's email address.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public required string Email { get; init; }

    /// <summary>
    /// The admin's plain-text password, as typed into the login form.
    /// </summary>
    [Required]
    public required string Password { get; init; }
}
