using Trust.Api.Domain.Admin;

namespace Trust.Api.Application.Repositories;

/// <summary>
/// Data access for <see cref="Event"/> records. Contains only reads and
/// writes — publishing rules and slug generation live in the service layer.
/// </summary>
public interface IEventRepository
{
    /// <summary>
    /// Finds an event by id, excluding soft-deleted rows.
    /// </summary>
    /// <param name="id">The event's id.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching event, or <see langword="null"/> if none exists.</returns>
    Task<Event?> GetByIdAsync(long id, CancellationToken cancellationToken);

    /// <summary>
    /// Finds an event by its unique slug, excluding soft-deleted rows.
    /// </summary>
    /// <param name="slug">The URL-friendly identifier to search for.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching event, or <see langword="null"/> if none exists.</returns>
    Task<Event?> GetBySlugAsync(string slug, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves one page of events, optionally filtered by published state,
    /// by whether the event date is in the future, and by a case-insensitive
    /// title search, ordered by event date.
    /// </summary>
    /// <param name="isPublished">When provided, only events with this published state are returned.</param>
    /// <param name="isUpcoming">
    /// When <see langword="true"/>, only events whose date is now or later are returned.
    /// When <see langword="false"/>, only events whose date has already passed are returned.
    /// When <see langword="null"/>, events are not filtered by date.
    /// </param>
    /// <param name="searchTitle">When provided, only events whose title contains this text (case-insensitive) are returned.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The maximum number of events per page.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching events for the page, and the total count across all pages.</returns>
    Task<(IReadOnlyList<Event> Items, int TotalCount)> GetPagedAsync(
        bool? isPublished,
        bool? isUpcoming,
        string? searchTitle,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Inserts a new event.
    /// </summary>
    /// <param name="eventItem">The event to insert.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task AddAsync(Event eventItem, CancellationToken cancellationToken);

    /// <summary>
    /// Persists changes made to an existing event (including edits and
    /// soft deletes, which the service expresses by setting <c>DeletedAt</c>).
    /// </summary>
    /// <param name="eventItem">The event, with its properties already updated by the caller.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task UpdateAsync(Event eventItem, CancellationToken cancellationToken);
}
