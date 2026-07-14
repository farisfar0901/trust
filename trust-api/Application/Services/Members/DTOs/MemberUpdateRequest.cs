using System.ComponentModel.DataAnnotations;

namespace Trust.Api.Application.Services.Members.DTOs;

/// <summary>
/// Replacement details for an existing member, as submitted by an admin.
/// </summary>
public sealed class MemberUpdateRequest
{
    /// <summary>
    /// The member's full name.
    /// </summary>
    [Required]
    [MaxLength(150)]
    public required string FullName { get; init; }

    /// <summary>
    /// The member's email address. Must be unique among active members.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public required string Email { get; init; }

    /// <summary>
    /// The member's phone number.
    /// </summary>
    [Required]
    [MaxLength(20)]
    public required string Phone { get; init; }

    /// <summary>
    /// The member's postal address, if provided.
    /// </summary>
    public string? Address { get; init; }

    /// <summary>
    /// The kind of membership: <c>General</c>, <c>Lifetime</c>, <c>Honorary</c>, or <c>Patron</c>.
    /// </summary>
    [Required]
    [RegularExpression(
        "^(General|Lifetime|Honorary|Patron)$",
        ErrorMessage = "MembershipType must be one of 'General', 'Lifetime', 'Honorary', 'Patron'.")]
    public required string MembershipType { get; init; }

    /// <summary>
    /// The member's status: <c>Active</c> or <c>Inactive</c>.
    /// </summary>
    [Required]
    [RegularExpression("^(Active|Inactive)$", ErrorMessage = "Status must be 'Active' or 'Inactive'.")]
    public required string Status { get; init; }

    /// <summary>
    /// The date the member joined.
    /// </summary>
    [Required]
    public required DateOnly JoinedDate { get; init; }

    /// <summary>
    /// The URL of the member's photo, if provided.
    /// </summary>
    [MaxLength(500)]
    public string? PhotoUrl { get; init; }
}
