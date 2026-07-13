using Trust.Api.Application.Common;
using Trust.Api.Application.Services.Gallery.DTOs;

namespace Trust.Api.Application.Services.Gallery;

/// <summary>
/// Business logic for creating, reading, updating, and soft-deleting gallery
/// media (photos and videos). The same operations serve both the
/// event-scoped Event Gallery screen and the general Gallery Management
/// screen, distinguished by whether an event id is supplied.
/// </summary>
public interface IGalleryMediaService
{
    /// <summary>
    /// Adds a new photo or video, validating that the referenced event
    /// exists when one is specified.
    /// </summary>
    /// <param name="request">The media item's details.</param>
    /// <param name="creatingAdminId">The id of the admin uploading the media.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The newly created media item.</returns>
    /// <exception cref="ArgumentException"><paramref name="request"/> references an event that does not exist.</exception>
    Task<GalleryMediaResponse> CreateAsync(
        GalleryMediaCreateRequest request, long creatingAdminId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a single media item by id.
    /// </summary>
    /// <param name="id">The media item's id.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching media item, or <see langword="null"/> if none exists.</returns>
    Task<GalleryMediaResponse?> GetByIdAsync(long id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves one page of media, optionally filtered by event and
    /// published state, and by a search over title and caption. Passing an
    /// <paramref name="eventId"/> gives the Event Gallery view; leaving it
    /// <see langword="null"/> gives the general Gallery Management view.
    /// </summary>
    /// <param name="eventId">When provided, only media linked to this event is returned.</param>
    /// <param name="isPublished">When provided, only media with this published state is returned.</param>
    /// <param name="searchText">When provided, only media whose title or caption contains this text is returned.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The maximum number of items per page.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The requested page of results.</returns>
    Task<PagedResult<GalleryMediaResponse>> GetPagedAsync(
        long? eventId,
        bool? isPublished,
        string? searchText,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Replaces an existing media item's details.
    /// </summary>
    /// <param name="id">The media item's id.</param>
    /// <param name="request">The media item's new details.</param>
    /// <param name="updatingAdminId">The id of the admin making the change.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The updated media item, or <see langword="null"/> if it does not exist.</returns>
    /// <exception cref="ArgumentException"><paramref name="request"/> references an event that does not exist.</exception>
    Task<GalleryMediaResponse?> UpdateAsync(
        long id, GalleryMediaUpdateRequest request, long updatingAdminId, CancellationToken cancellationToken);

    /// <summary>
    /// Soft-deletes a media item so it no longer appears in normal listings.
    /// </summary>
    /// <param name="id">The media item's id.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns><see langword="true"/> if the media item was found and deleted; otherwise <see langword="false"/>.</returns>
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
}
