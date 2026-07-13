using Trust.Api.Domain.Admin;

namespace Trust.Api.Application.Repositories;

/// <summary>
/// Data access for <see cref="GalleryMedia"/> records. The same table backs
/// both the general Gallery Management screen and the event-scoped Event
/// Gallery screen; callers distinguish the two by passing an event id
/// filter or leaving it <see langword="null"/>.
/// </summary>
public interface IGalleryMediaRepository
{
    /// <summary>
    /// Finds a gallery media item by id, excluding soft-deleted rows.
    /// </summary>
    /// <param name="id">The media item's id.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching media item, or <see langword="null"/> if none exists.</returns>
    Task<GalleryMedia?> GetByIdAsync(long id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves one page of gallery media, optionally filtered by event and
    /// published state, and by a case-insensitive search over title and
    /// caption, ordered for display.
    /// </summary>
    /// <param name="eventId">When provided, only media linked to this event is returned.</param>
    /// <param name="isPublished">When provided, only media with this published state is returned.</param>
    /// <param name="searchText">When provided, only media whose title or caption contains this text (case-insensitive) is returned.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The maximum number of items per page.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching media items for the page, and the total count across all pages.</returns>
    Task<(IReadOnlyList<GalleryMedia> Items, int TotalCount)> GetPagedAsync(
        long? eventId,
        bool? isPublished,
        string? searchText,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Inserts a new gallery media item.
    /// </summary>
    /// <param name="galleryMedia">The media item to insert.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task AddAsync(GalleryMedia galleryMedia, CancellationToken cancellationToken);

    /// <summary>
    /// Persists changes made to an existing gallery media item (including
    /// edits and soft deletes, which the service expresses by setting
    /// <c>DeletedAt</c>).
    /// </summary>
    /// <param name="galleryMedia">The media item, with its properties already updated by the caller.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task UpdateAsync(GalleryMedia galleryMedia, CancellationToken cancellationToken);
}
