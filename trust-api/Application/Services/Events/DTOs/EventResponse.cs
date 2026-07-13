namespace Trust.Api.Application.Services.Events.DTOs;

/// <summary>
/// An event as shown to an admin or on the public site.
/// </summary>
public sealed class EventResponse
{
    /// <summary>
    /// The event's id.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// The event's title.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// The event's URL-friendly identifier.
    /// </summary>
    public required string Slug { get; init; }

    /// <summary>
    /// The event's category, if any.
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// A longer description of the event, if any.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Where the event takes place, if provided.
    /// </summary>
    public string? Location { get; init; }

    /// <summary>
    /// The date and time the event takes place.
    /// </summary>
    public required DateTimeOffset EventDate { get; init; }

    /// <summary>
    /// Whether <see cref="EventDate"/> is still in the future. Computed at
    /// read time rather than stored, so it can never go stale.
    /// </summary>
    public required bool IsUpcoming { get; init; }

    /// <summary>
    /// Whether the event is visible on the public site.
    /// </summary>
    public required bool IsPublished { get; init; }

    /// <summary>
    /// The URL of the event's cover image, if any.
    /// </summary>
    public string? CoverImageUrl { get; init; }

    /// <summary>
    /// The moment the event was created.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// The moment the event was last updated.
    /// </summary>
    public required DateTimeOffset UpdatedAt { get; init; }
}
