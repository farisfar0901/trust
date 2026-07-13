using Trust.Api.Application.Common;
using Trust.Api.Application.Services.Events.DTOs;

namespace Trust.Api.Application.Services.Events;

/// <summary>
/// Business logic for creating, reading, updating, and soft-deleting events.
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Creates a new event, generating a unique slug automatically if one
    /// was not supplied.
    /// </summary>
    /// <param name="request">The event's details.</param>
    /// <param name="creatingAdminId">The id of the admin creating the event.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The newly created event.</returns>
    Task<EventResponse> CreateAsync(EventCreateRequest request, long creatingAdminId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a single event by id.
    /// </summary>
    /// <param name="id">The event's id.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching event, or <see langword="null"/> if none exists.</returns>
    Task<EventResponse?> GetByIdAsync(long id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves one page of events, optionally filtered by published state,
    /// by whether the event is upcoming or already completed, and by a
    /// case-insensitive title search.
    /// </summary>
    /// <param name="isPublished">When provided, only events with this published state are returned.</param>
    /// <param name="isUpcoming">
    /// When <see langword="true"/>, only upcoming events are returned. When
    /// <see langword="false"/>, only completed (past) events are returned.
    /// When <see langword="null"/>, events are not filtered by date.
    /// </param>
    /// <param name="searchTitle">When provided, only events whose title contains this text are returned.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The maximum number of events per page.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The requested page of results.</returns>
    Task<PagedResult<EventResponse>> GetPagedAsync(
        bool? isPublished,
        bool? isUpcoming,
        string? searchTitle,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Replaces an existing event's details.
    /// </summary>
    /// <param name="id">The event's id.</param>
    /// <param name="request">The event's new details.</param>
    /// <param name="updatingAdminId">The id of the admin making the change.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The updated event, or <see langword="null"/> if it does not exist.</returns>
    Task<EventResponse?> UpdateAsync(
        long id, EventUpdateRequest request, long updatingAdminId, CancellationToken cancellationToken);

    /// <summary>
    /// Soft-deletes an event so it no longer appears in normal listings.
    /// </summary>
    /// <param name="id">The event's id.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns><see langword="true"/> if the event was found and deleted; otherwise <see langword="false"/>.</returns>
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
}
