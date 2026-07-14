namespace Trust.Api.Application.Services.Members.DTOs;

/// <summary>
/// A member as shown to an admin.
/// </summary>
public sealed class MemberResponse
{
    /// <summary>
    /// The member's id.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// The member's full name.
    /// </summary>
    public required string FullName { get; init; }

    /// <summary>
    /// The member's email address.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// The member's phone number.
    /// </summary>
    public required string Phone { get; init; }

    /// <summary>
    /// The member's postal address, if any.
    /// </summary>
    public string? Address { get; init; }

    /// <summary>
    /// The kind of membership: <c>General</c>, <c>Lifetime</c>, <c>Honorary</c>, or <c>Patron</c>.
    /// </summary>
    public required string MembershipType { get; init; }

    /// <summary>
    /// The member's status: <c>Active</c> or <c>Inactive</c>.
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    /// The date the member joined.
    /// </summary>
    public required DateOnly JoinedDate { get; init; }

    /// <summary>
    /// The URL of the member's photo, if any.
    /// </summary>
    public string? PhotoUrl { get; init; }

    /// <summary>
    /// The moment the member record was created.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// The moment the member record was last updated.
    /// </summary>
    public required DateTimeOffset UpdatedAt { get; init; }
}
