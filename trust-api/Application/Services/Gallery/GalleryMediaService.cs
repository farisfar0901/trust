using Trust.Api.Application.Common;
using Trust.Api.Application.Repositories;
using Trust.Api.Application.Services.Gallery.DTOs;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Application.Services.Gallery;

/// <summary>
/// Default <see cref="IGalleryMediaService"/> implementation.
/// </summary>
public sealed class GalleryMediaService : IGalleryMediaService
{
    private readonly IGalleryMediaRepository _galleryMediaRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>
    /// Creates the service.
    /// </summary>
    public GalleryMediaService(
        IGalleryMediaRepository galleryMediaRepository,
        IEventRepository eventRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _galleryMediaRepository = galleryMediaRepository;
        _eventRepository = eventRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    /// <inheritdoc />
    public async Task<GalleryMediaResponse> CreateAsync(
        GalleryMediaCreateRequest request, long creatingAdminId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        await EnsureEventExistsAsync(request.EventId, cancellationToken);

        var galleryMedia = new GalleryMedia
        {
            EventId = request.EventId,
            Title = request.Title,
            Caption = request.Caption,
            MediaUrl = request.MediaUrl,
            MediaType = request.MediaType,
            MimeType = request.MimeType,
            FileSizeBytes = request.FileSizeBytes,
            Year = request.Year,
            TakenAt = request.TakenAt,
            DisplayOrder = request.DisplayOrder,
            IsPublished = request.IsPublished,
            CreatedByAdminId = creatingAdminId,
            UpdatedByAdminId = creatingAdminId,
        };

        await _galleryMediaRepository.AddAsync(galleryMedia, cancellationToken);
        return ToResponse(galleryMedia);
    }

    /// <inheritdoc />
    public async Task<GalleryMediaResponse?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var galleryMedia = await _galleryMediaRepository.GetByIdAsync(id, cancellationToken);
        return galleryMedia is null ? null : ToResponse(galleryMedia);
    }

    /// <inheritdoc />
    public async Task<PagedResult<GalleryMediaResponse>> GetPagedAsync(
        long? eventId,
        bool? isPublished,
        string? searchText,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _galleryMediaRepository.GetPagedAsync(
            eventId, isPublished, searchText, pageNumber, pageSize, cancellationToken);

        return new PagedResult<GalleryMediaResponse>
        {
            Items = items.Select(ToResponse).ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    /// <inheritdoc />
    public async Task<GalleryMediaResponse?> UpdateAsync(
        long id, GalleryMediaUpdateRequest request, long updatingAdminId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var galleryMedia = await _galleryMediaRepository.GetByIdAsync(id, cancellationToken);
        if (galleryMedia is null)
        {
            return null;
        }

        await EnsureEventExistsAsync(request.EventId, cancellationToken);

        galleryMedia.EventId = request.EventId;
        galleryMedia.Title = request.Title;
        galleryMedia.Caption = request.Caption;
        galleryMedia.MediaUrl = request.MediaUrl;
        galleryMedia.MediaType = request.MediaType;
        galleryMedia.MimeType = request.MimeType;
        galleryMedia.FileSizeBytes = request.FileSizeBytes;
        galleryMedia.Year = request.Year;
        galleryMedia.TakenAt = request.TakenAt;
        galleryMedia.DisplayOrder = request.DisplayOrder;
        galleryMedia.IsPublished = request.IsPublished;
        galleryMedia.UpdatedByAdminId = updatingAdminId;

        await _galleryMediaRepository.UpdateAsync(galleryMedia, cancellationToken);
        return ToResponse(galleryMedia);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var galleryMedia = await _galleryMediaRepository.GetByIdAsync(id, cancellationToken);
        if (galleryMedia is null)
        {
            return false;
        }

        galleryMedia.DeletedAt = _dateTimeProvider.UtcNow;
        await _galleryMediaRepository.UpdateAsync(galleryMedia, cancellationToken);
        return true;
    }

    /// <summary>
    /// Verifies that a referenced event exists before linking media to it.
    /// Does nothing when <paramref name="eventId"/> is <see langword="null"/>,
    /// since that means "general gallery, not linked to any event".
    /// </summary>
    /// <exception cref="ArgumentException">The event does not exist.</exception>
    private async Task EnsureEventExistsAsync(long? eventId, CancellationToken cancellationToken)
    {
        if (eventId is null)
        {
            return;
        }

        var eventItem = await _eventRepository.GetByIdAsync(eventId.Value, cancellationToken);
        if (eventItem is null)
        {
            throw new ArgumentException($"Event with id {eventId.Value} was not found.", nameof(eventId));
        }
    }

    /// <summary>
    /// Maps a <see cref="GalleryMedia"/> entity to its response DTO.
    /// </summary>
    private static GalleryMediaResponse ToResponse(GalleryMedia galleryMedia) => new()
    {
        Id = galleryMedia.Id,
        EventId = galleryMedia.EventId,
        Title = galleryMedia.Title,
        Caption = galleryMedia.Caption,
        MediaUrl = galleryMedia.MediaUrl,
        MediaType = galleryMedia.MediaType,
        MimeType = galleryMedia.MimeType,
        FileSizeBytes = galleryMedia.FileSizeBytes,
        Year = galleryMedia.Year,
        TakenAt = galleryMedia.TakenAt,
        DisplayOrder = galleryMedia.DisplayOrder,
        IsPublished = galleryMedia.IsPublished,
        CreatedAt = galleryMedia.CreatedAt,
        UpdatedAt = galleryMedia.UpdatedAt,
    };
}
