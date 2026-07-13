namespace Trust.Api.Application.Services.Gallery.DTOs;

/// <summary>
/// A photo or video as shown to an admin or on the public site.
/// </summary>
public sealed class GalleryMediaResponse
{
    /// <summary>
    /// The media item's id.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// The event this media belongs to, or <see langword="null"/> if it is
    /// only in the general gallery library.
    /// </summary>
    public long? EventId { get; init; }

    /// <summary>
    /// The media item's title, if any.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// A caption describing the media item, if any.
    /// </summary>
    public string? Caption { get; init; }

    /// <summary>
    /// The URL where the uploaded file can be accessed.
    /// </summary>
    public required string MediaUrl { get; init; }

    /// <summary>
    /// The kind of media: <c>Photo</c> or <c>Video</c>.
    /// </summary>
    public required string MediaType { get; init; }

    /// <summary>
    /// The uploaded file's MIME type, if known.
    /// </summary>
    public string? MimeType { get; init; }

    /// <summary>
    /// The uploaded file's size in bytes, if known.
    /// </summary>
    public long? FileSizeBytes { get; init; }

    /// <summary>
    /// The year the media relates to.
    /// </summary>
    public required short Year { get; init; }

    /// <summary>
    /// The exact date the media was taken, if known.
    /// </summary>
    public DateOnly? TakenAt { get; init; }

    /// <summary>
    /// The item's position within its gallery, lowest first.
    /// </summary>
    public required int DisplayOrder { get; init; }

    /// <summary>
    /// Whether the media is visible on the public site.
    /// </summary>
    public required bool IsPublished { get; init; }

    /// <summary>
    /// The moment the media item was created.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// The moment the media item was last updated.
    /// </summary>
    public required DateTimeOffset UpdatedAt { get; init; }
}
