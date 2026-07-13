using Microsoft.AspNetCore.Mvc;
using Trust.Api.Application.Common;
using Trust.Api.Application.Services.Gallery;
using Trust.Api.Application.Services.Gallery.DTOs;
using Trust.Api.Authorization;

namespace Trust.Api.Controllers.Admin;

/// <summary>
/// HTTP endpoints for admins to manage gallery media (photos and videos).
/// This controller only translates HTTP requests into calls on
/// <see cref="IGalleryMediaService"/> and maps the results to HTTP
/// responses — all event-existence validation and soft-delete logic lives
/// in the service layer. The same endpoints serve both the event-scoped
/// Event Gallery screen (pass <c>eventId</c>) and the general Gallery
/// Management screen (omit it).
/// </summary>
[ApiController]
[Route("api/admin/gallery")]
[RequireAdminRole("SuperAdmin", "Admin")]
public sealed class AdminGalleryController : ControllerBase
{
    /// <summary>
    /// Largest page size a caller may request, to avoid accidentally loading
    /// the entire table in one response.
    /// </summary>
    private const int MaxPageSize = 100;

    private readonly IGalleryMediaService _galleryMediaService;

    /// <summary>
    /// Creates the controller.
    /// </summary>
    /// <param name="galleryMediaService">The service that reads, writes, and soft-deletes gallery media.</param>
    public AdminGalleryController(IGalleryMediaService galleryMediaService)
    {
        _galleryMediaService = galleryMediaService;
    }

    /// <summary>
    /// Retrieves one page of gallery media, optionally scoped to a single
    /// event and filtered by a title/caption search.
    /// </summary>
    /// <param name="eventId">When provided, only media linked to this event is returned (the Event Gallery view). Omit for the general Gallery Management view.</param>
    /// <param name="search">When provided, only media whose title or caption contains this text (case-insensitive) is returned.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The maximum number of items per page (1-100). Defaults to 20.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GalleryMediaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] long? eventId = null,
        [FromQuery] string? search = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var result = await _galleryMediaService.GetPagedAsync(
            eventId, isPublished: null, search, pageNumber, pageSize, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single gallery media item by id.
    /// </summary>
    /// <param name="id">The media item's id.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(GalleryMediaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var galleryMedia = await _galleryMediaService.GetByIdAsync(id, cancellationToken);
        return galleryMedia is null ? NotFound() : Ok(galleryMedia);
    }

    /// <summary>
    /// Adds a new photo or video.
    /// </summary>
    /// <param name="request">The media item's details.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpPost]
    [ProducesResponseType(typeof(GalleryMediaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] GalleryMediaCreateRequest request, CancellationToken cancellationToken)
    {
        var creatingAdminId = User.GetAdminId();

        try
        {
            var created = await _galleryMediaService.CreateAsync(request, creatingAdminId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            // The service throws this when the request references an event
            // that does not exist — a client input problem, not a server error.
            return ValidationProblem(ex.Message);
        }
    }

    /// <summary>
    /// Replaces an existing gallery media item's details.
    /// </summary>
    /// <param name="id">The media item's id.</param>
    /// <param name="request">The media item's new details.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(GalleryMediaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long id, [FromBody] GalleryMediaUpdateRequest request, CancellationToken cancellationToken)
    {
        var updatingAdminId = User.GetAdminId();

        try
        {
            var updated = await _galleryMediaService.UpdateAsync(id, request, updatingAdminId, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    /// <summary>
    /// Soft-deletes a gallery media item so it no longer appears in normal
    /// listings. The underlying row is kept (<c>deleted_at</c> is set),
    /// never removed.
    /// </summary>
    /// <param name="id">The media item's id.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deleted = await _galleryMediaService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
