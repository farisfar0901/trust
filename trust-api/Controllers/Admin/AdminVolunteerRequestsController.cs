using Microsoft.AspNetCore.Mvc;
using Trust.Api.Application.Common;
using Trust.Api.Application.Services.Volunteers;
using Trust.Api.Application.Services.Volunteers.DTOs;
using Trust.Api.Authorization;

namespace Trust.Api.Controllers.Admin;

/// <summary>
/// HTTP endpoints for admins to review volunteer applications. This
/// controller only translates HTTP requests into calls on
/// <see cref="IVolunteerRequestService"/> and maps the results to HTTP
/// responses — all the review and promotion logic lives in the service
/// layer.
/// </summary>
[ApiController]
[Route("api/admin/volunteer-requests")]
[RequireAdminRole("SuperAdmin", "Admin")]
public sealed class AdminVolunteerRequestsController : ControllerBase
{
    /// <summary>
    /// Largest page size a caller may request, to avoid accidentally loading
    /// the entire table in one response.
    /// </summary>
    private const int MaxPageSize = 100;

    private const string ApprovedStatus = "Approved";
    private const string RejectedStatus = "Rejected";

    private readonly IVolunteerRequestService _volunteerRequestService;

    /// <summary>
    /// Creates the controller.
    /// </summary>
    /// <param name="volunteerRequestService">The service that reads and reviews volunteer requests.</param>
    public AdminVolunteerRequestsController(IVolunteerRequestService volunteerRequestService)
    {
        _volunteerRequestService = volunteerRequestService;
    }

    /// <summary>
    /// Retrieves one page of volunteer requests, optionally filtered by status.
    /// </summary>
    /// <param name="status">When provided, only requests with this status (<c>Pending</c>, <c>Approved</c>, or <c>Rejected</c>) are returned.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The maximum number of requests per page (1-100). Defaults to 20.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<VolunteerRequestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] string? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var result = await _volunteerRequestService.GetPagedAsync(status, pageNumber, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single volunteer request by id.
    /// </summary>
    /// <param name="id">The request's id.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(VolunteerRequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var volunteerRequest = await _volunteerRequestService.GetByIdAsync(id, cancellationToken);
        return volunteerRequest is null ? NotFound() : Ok(volunteerRequest);
    }

    /// <summary>
    /// Approves a pending volunteer request, promoting the applicant to an
    /// active volunteer.
    /// </summary>
    /// <param name="id">The request's id.</param>
    /// <param name="reviewNote">An optional internal note explaining the decision.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpPost("{id:long}/approve")]
    [ProducesResponseType(typeof(VolunteerRequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Approve(long id, [FromQuery] string? reviewNote, CancellationToken cancellationToken)
        => ReviewAsync(id, ApprovedStatus, reviewNote, cancellationToken);

    /// <summary>
    /// Rejects a pending volunteer request.
    /// </summary>
    /// <param name="id">The request's id.</param>
    /// <param name="reviewNote">An optional internal note explaining the decision.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpPost("{id:long}/reject")]
    [ProducesResponseType(typeof(VolunteerRequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<IActionResult> Reject(long id, [FromQuery] string? reviewNote, CancellationToken cancellationToken)
        => ReviewAsync(id, RejectedStatus, reviewNote, cancellationToken);

    /// <summary>
    /// Shared implementation for <see cref="Approve"/> and <see cref="Reject"/>:
    /// builds the review request with the appropriate status and delegates
    /// to <see cref="IVolunteerRequestService.ReviewAsync"/>.
    /// </summary>
    private async Task<IActionResult> ReviewAsync(
        long id, string status, string? reviewNote, CancellationToken cancellationToken)
    {
        var reviewingAdminId = User.GetAdminId();
        var request = new VolunteerRequestReviewRequest { Status = status, ReviewNote = reviewNote };

        try
        {
            var updated = await _volunteerRequestService.ReviewAsync(id, request, reviewingAdminId, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            // The service throws this when the request has already been
            // reviewed — a conflict with the current resource state, not a
            // server error.
            return Conflict(new { message = ex.Message });
        }
    }
}
