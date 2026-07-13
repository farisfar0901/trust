using System.ComponentModel.DataAnnotations;

namespace Trust.Api.Application.Services.Gallery.DTOs;

/// <summary>
/// Details for a new photo or video, as submitted by an admin. The same
/// shape is used whether the item belongs to a specific event (Event
/// Gallery) or the general library (Gallery Management) — set
/// <see cref="EventId"/> or leave it <see langword="null"/> accordingly.
/// </summary>
public sealed class GalleryMediaCreateRequest
{
    /// <summary>
    /// The event this media belongs to, or <see langword="null"/> for the
    /// general gallery library.
    /// </summary>
    public long? EventId { get; init; }

    /// <summary>
    /// The media item's title, if provided.
    /// </summary>
    [MaxLength(200)]
    public string? Title { get; init; }

    /// <summary>
    /// A caption describing the media item, if provided.
    /// </summary>
    public string? Caption { get; init; }

    /// <summary>
    /// The URL where the uploaded file can be accessed.
    /// </summary>
    [Required]
    [MaxLength(500)]
    public required string MediaUrl { get; init; }

    /// <summary>
    /// The kind of media: <c>Photo</c> or <c>Video</c>.
    /// </summary>
    [Required]
    [RegularExpression("^(Photo|Video)$", ErrorMessage = "MediaType must be 'Photo' or 'Video'.")]
    public required string MediaType { get; init; }

    /// <summary>
    /// The uploaded file's MIME type, if known.
    /// </summary>
    [MaxLength(100)]
    public string? MimeType { get; init; }

    /// <summary>
    /// The uploaded file's size in bytes, if known.
    /// </summary>
    public long? FileSizeBytes { get; init; }

    /// <summary>
    /// The year the media relates to. Kept even when <see cref="TakenAt"/>
    /// is unknown, since the year is often known on its own.
    /// </summary>
    [Required]
    [Range(2000, 2100)]
    public required short Year { get; init; }

    /// <summary>
    /// The exact date the media was taken, if known.
    /// </summary>
    public DateOnly? TakenAt { get; init; }

    /// <summary>
    /// The item's position within its gallery, lowest first. Defaults to 0.
    /// </summary>
    public int DisplayOrder { get; init; }

    /// <summary>
    /// Whether the media should be visible on the public site immediately.
    /// Defaults to <see langword="true"/>.
    /// </summary>
    public bool IsPublished { get; init; } = true;
}
