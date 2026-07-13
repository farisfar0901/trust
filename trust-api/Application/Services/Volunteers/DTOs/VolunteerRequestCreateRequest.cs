using System.ComponentModel.DataAnnotations;

namespace Trust.Api.Application.Services.Volunteers.DTOs;

/// <summary>
/// A volunteer application submitted by a member of the public.
/// </summary>
public sealed class VolunteerRequestCreateRequest
{
    /// <summary>
    /// The applicant's full name.
    /// </summary>
    [Required]
    [MaxLength(150)]
    public required string FullName { get; init; }

    /// <summary>
    /// The applicant's email address.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public required string Email { get; init; }

    /// <summary>
    /// The applicant's phone number.
    /// </summary>
    [Required]
    [MaxLength(20)]
    public required string Phone { get; init; }

    /// <summary>
    /// The applicant's city, if provided.
    /// </summary>
    [MaxLength(100)]
    public string? City { get; init; }

    /// <summary>
    /// The area of volunteering the applicant is interested in, if provided.
    /// </summary>
    [MaxLength(150)]
    public string? PreferredArea { get; init; }

    /// <summary>
    /// A free-text message from the applicant, if provided.
    /// </summary>
    public string? Message { get; init; }
}
