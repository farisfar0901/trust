namespace Trust.Api.Application.Services.Volunteers.DTOs;

/// <summary>
/// A volunteer application as shown to an admin, including its review state.
/// </summary>
public sealed class VolunteerRequestResponse
{
    /// <summary>
    /// The request's id.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// The applicant's full name.
    /// </summary>
    public required string FullName { get; init; }

    /// <summary>
    /// The applicant's email address.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// The applicant's phone number.
    /// </summary>
    public required string Phone { get; init; }

    /// <summary>
    /// The applicant's city, if provided.
    /// </summary>
    public string? City { get; init; }

    /// <summary>
    /// The area of volunteering the applicant is interested in, if provided.
    /// </summary>
    public string? PreferredArea { get; init; }

    /// <summary>
    /// A free-text message from the applicant, if provided.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// The request's review status: <c>Pending</c>, <c>Approved</c>, or <c>Rejected</c>.
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    /// The reviewing admin's internal note, if the request has been reviewed.
    /// </summary>
    public string? ReviewNote { get; init; }

    /// <summary>
    /// The id of the admin who reviewed the request, if it has been reviewed.
    /// </summary>
    public long? ReviewedByAdminId { get; init; }

    /// <summary>
    /// The moment the request was reviewed, if it has been reviewed.
    /// </summary>
    public DateTimeOffset? ReviewedAt { get; init; }

    /// <summary>
    /// The moment the request was submitted.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }
}
