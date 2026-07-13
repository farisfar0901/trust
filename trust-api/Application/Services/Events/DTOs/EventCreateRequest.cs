using System.ComponentModel.DataAnnotations;

namespace Trust.Api.Application.Services.Events.DTOs;

/// <summary>
/// Details for a new event, as submitted by an admin.
/// </summary>
public sealed class EventCreateRequest
{
    /// <summary>
    /// The event's title.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public required string Title { get; init; }

    /// <summary>
    /// A URL-friendly identifier for the event. When left empty, one is
    /// generated automatically from the title.
    /// </summary>
    [MaxLength(220)]
    public string? Slug { get; init; }

    /// <summary>
    /// The event's category, if provided.
    /// </summary>
    [MaxLength(100)]
    public string? Category { get; init; }

    /// <summary>
    /// A longer description of the event, if provided.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Where the event takes place, if provided.
    /// </summary>
    [MaxLength(255)]
    public string? Location { get; init; }

    /// <summary>
    /// The date and time the event takes place.
    /// </summary>
    [Required]
    public required DateTimeOffset EventDate { get; init; }

    /// <summary>
    /// Whether the event should be visible on the public site immediately.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool IsPublished { get; init; } = true;

    /// <summary>
    /// The URL of the event's cover image, if provided.
    /// </summary>
    [MaxLength(500)]
    public string? CoverImageUrl { get; init; }
}
