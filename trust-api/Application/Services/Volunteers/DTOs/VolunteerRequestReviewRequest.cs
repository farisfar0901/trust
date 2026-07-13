using System.ComponentModel.DataAnnotations;

namespace Trust.Api.Application.Services.Volunteers.DTOs;

/// <summary>
/// An admin's decision on a pending volunteer application.
/// </summary>
public sealed class VolunteerRequestReviewRequest
{
    /// <summary>
    /// The review outcome: either <c>Approved</c> or <c>Rejected</c>.
    /// </summary>
    [Required]
    [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Status must be 'Approved' or 'Rejected'.")]
    public required string Status { get; init; }

    /// <summary>
    /// The reviewing admin's internal note explaining the decision.
    /// </summary>
    public string? ReviewNote { get; init; }
}
