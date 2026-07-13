using Microsoft.AspNetCore.Mvc;
using Trust.Api.Application.Common;
using Trust.Api.Application.Services.Events;
using Trust.Api.Application.Services.Events.DTOs;
using Trust.Api.Authorization;

namespace Trust.Api.Controllers.Admin;

/// <summary>
/// HTTP endpoints for admins to manage events. This controller only
/// translates HTTP requests into calls on <see cref="IEventService"/> and
/// maps the results to HTTP responses — all slug generation, upcoming/past
/// computation, and soft-delete logic lives in the service layer.
/// </summary>
[ApiController]
[Route("api/admin/events")]
[RequireAdminRole("SuperAdmin", "Admin")]
public sealed class AdminEventsController : ControllerBase
{
    /// <summary>
    /// Largest page size a caller may request, to avoid accidentally loading
    /// the entire table in one response.
    /// </summary>
    private const int MaxPageSize = 100;

    private readonly IEventService _eventService;

    /// <summary>
    /// Creates the controller.
    /// </summary>
    /// <param name="eventService">The service that reads, writes, and soft-deletes events.</param>
    public AdminEventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    /// <summary>
    /// Retrieves one page of events, optionally filtered by whether they are
    /// upcoming or completed, and by a title search.
    /// </summary>
    /// <param name="filter">
    /// <c>Upcoming</c> for events whose date is still ahead, <c>Completed</c>
    /// for events whose date has passed, or <c>All</c> (the default) for both.
    /// </param>
    /// <param name="search">When provided, only events whose title contains this text (case-insensitive) are returned.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The maximum number of events per page (1-100). Defaults to 20.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EventResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] string filter = "All",
        [FromQuery] string? search = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        var isUpcoming = ParseEventFilter(filter);

        var result = await _eventService.GetPagedAsync(
            isPublished: null, isUpcoming, search, pageNumber, pageSize, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single event by id.
    /// </summary>
    /// <param name="id">The event's id.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var eventItem = await _eventService.GetByIdAsync(id, cancellationToken);
        return eventItem is null ? NotFound() : Ok(eventItem);
    }

    /// <summary>
    /// Creates a new event.
    /// </summary>
    /// <param name="request">The event's details.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpPost]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] EventCreateRequest request, CancellationToken cancellationToken)
    {
        var creatingAdminId = User.GetAdminId();
        var created = await _eventService.CreateAsync(request, creatingAdminId, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Replaces an existing event's details.
    /// </summary>
    /// <param name="id">The event's id.</param>
    /// <param name="request">The event's new details.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long id, [FromBody] EventUpdateRequest request, CancellationToken cancellationToken)
    {
        var updatingAdminId = User.GetAdminId();
        var updated = await _eventService.UpdateAsync(id, request, updatingAdminId, cancellationToken);

        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    /// Soft-deletes an event so it no longer appears in normal listings. The
    /// underlying row is kept (<c>deleted_at</c> is set), never removed.
    /// </summary>
    /// <param name="id">The event's id.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deleted = await _eventService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    /// <summary>
    /// Converts the <c>filter</c> query value into the <c>isUpcoming</c>
    /// parameter the service expects: <c>Upcoming</c> → <see langword="true"/>,
    /// <c>Completed</c> → <see langword="false"/>, anything else (including
    /// <c>All</c>) → <see langword="null"/> (no date filtering).
    /// </summary>
    private static bool? ParseEventFilter(string filter) => filter.Trim().ToLowerInvariant() switch
    {
        "upcoming" => true,
        "completed" => false,
        _ => null,
    };
}
