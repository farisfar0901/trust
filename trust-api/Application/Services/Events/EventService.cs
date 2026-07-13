using Trust.Api.Application.Common;
using Trust.Api.Application.Repositories;
using Trust.Api.Application.Services.Events.DTOs;
using Trust.Api.Domain.Admin;
using Trust.Api.Infrastructure;

namespace Trust.Api.Application.Services.Events;

/// <summary>
/// Default <see cref="IEventService"/> implementation.
/// </summary>
public sealed class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>
    /// Creates the service.
    /// </summary>
    public EventService(IEventRepository eventRepository, IDateTimeProvider dateTimeProvider)
    {
        _eventRepository = eventRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    /// <inheritdoc />
    public async Task<EventResponse> CreateAsync(
        EventCreateRequest request, long creatingAdminId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var slug = await GenerateUniqueSlugAsync(request.Title, request.Slug, excludeEventId: null, cancellationToken);

        var eventItem = new Event
        {
            Title = request.Title,
            Slug = slug,
            Category = request.Category,
            Description = request.Description,
            Location = request.Location,
            EventDate = request.EventDate,
            IsPublished = request.IsPublished,
            CoverImageUrl = request.CoverImageUrl,
            CreatedByAdminId = creatingAdminId,
            UpdatedByAdminId = creatingAdminId,
        };

        await _eventRepository.AddAsync(eventItem, cancellationToken);
        return ToResponse(eventItem);
    }

    /// <inheritdoc />
    public async Task<EventResponse?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var eventItem = await _eventRepository.GetByIdAsync(id, cancellationToken);
        return eventItem is null ? null : ToResponse(eventItem);
    }

    /// <inheritdoc />
    public async Task<PagedResult<EventResponse>> GetPagedAsync(
        bool? isPublished,
        bool? isUpcoming,
        string? searchTitle,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _eventRepository.GetPagedAsync(
            isPublished, isUpcoming, searchTitle, pageNumber, pageSize, cancellationToken);

        return new PagedResult<EventResponse>
        {
            Items = items.Select(ToResponse).ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    /// <inheritdoc />
    public async Task<EventResponse?> UpdateAsync(
        long id, EventUpdateRequest request, long updatingAdminId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var eventItem = await _eventRepository.GetByIdAsync(id, cancellationToken);
        if (eventItem is null)
        {
            return null;
        }

        var slug = await GenerateUniqueSlugAsync(request.Title, request.Slug, excludeEventId: id, cancellationToken);

        eventItem.Title = request.Title;
        eventItem.Slug = slug;
        eventItem.Category = request.Category;
        eventItem.Description = request.Description;
        eventItem.Location = request.Location;
        eventItem.EventDate = request.EventDate;
        eventItem.IsPublished = request.IsPublished;
        eventItem.CoverImageUrl = request.CoverImageUrl;
        eventItem.UpdatedByAdminId = updatingAdminId;

        await _eventRepository.UpdateAsync(eventItem, cancellationToken);
        return ToResponse(eventItem);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var eventItem = await _eventRepository.GetByIdAsync(id, cancellationToken);
        if (eventItem is null)
        {
            return false;
        }

        eventItem.DeletedAt = _dateTimeProvider.UtcNow;
        await _eventRepository.UpdateAsync(eventItem, cancellationToken);
        return true;
    }

    /// <summary>
    /// Builds a URL-friendly, unique slug from the requested slug or title,
    /// appending <c>-2</c>, <c>-3</c>, and so on if the base slug is already
    /// taken by a different event.
    /// </summary>
    /// <param name="title">The event's title, used when no slug is supplied.</param>
    /// <param name="requestedSlug">The admin-supplied slug, if any.</param>
    /// <param name="excludeEventId">
    /// The id of the event being updated, so it does not collide with itself.
    /// </param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    private async Task<string> GenerateUniqueSlugAsync(
        string title, string? requestedSlug, long? excludeEventId, CancellationToken cancellationToken)
    {
        var baseSlug = SlugHelper.ToSlug(string.IsNullOrWhiteSpace(requestedSlug) ? title : requestedSlug);

        var candidateSlug = baseSlug;
        var suffix = 1;

        while (true)
        {
            var existingEvent = await _eventRepository.GetBySlugAsync(candidateSlug, cancellationToken);
            if (existingEvent is null || existingEvent.Id == excludeEventId)
            {
                return candidateSlug;
            }

            suffix++;
            candidateSlug = $"{baseSlug}-{suffix}";
        }
    }

    /// <summary>
    /// Maps an <see cref="Event"/> entity to its response DTO, computing
    /// <see cref="EventResponse.IsUpcoming"/> from the current time rather
    /// than reading a stored value.
    /// </summary>
    private EventResponse ToResponse(Event eventItem) => new()
    {
        Id = eventItem.Id,
        Title = eventItem.Title,
        Slug = eventItem.Slug,
        Category = eventItem.Category,
        Description = eventItem.Description,
        Location = eventItem.Location,
        EventDate = eventItem.EventDate,
        IsUpcoming = eventItem.EventDate >= _dateTimeProvider.UtcNow,
        IsPublished = eventItem.IsPublished,
        CoverImageUrl = eventItem.CoverImageUrl,
        CreatedAt = eventItem.CreatedAt,
        UpdatedAt = eventItem.UpdatedAt,
    };
}
