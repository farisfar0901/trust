using Trust.Api.Application.Common;
using Trust.Api.Application.Services.Volunteers.DTOs;

namespace Trust.Api.Application.Services.Volunteers;

/// <summary>
/// Business logic for submitting and reviewing volunteer applications,
/// including promoting an approved application to an active volunteer.
/// </summary>
public interface IVolunteerRequestService
{
    /// <summary>
    /// Records a new volunteer application submitted by a member of the public.
    /// </summary>
    /// <param name="request">The applicant's details.</param>
    /// <param name="ipAddress">The submitter's IP address, recorded for abuse triage.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The newly created request.</returns>
    Task<VolunteerRequestResponse> SubmitAsync(
        VolunteerRequestCreateRequest request, string? ipAddress, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a single volunteer request by id.
    /// </summary>
    /// <param name="id">The request's id.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching request, or <see langword="null"/> if none exists.</returns>
    Task<VolunteerRequestResponse?> GetByIdAsync(long id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves one page of volunteer requests, optionally filtered by status.
    /// </summary>
    /// <param name="status">When provided, only requests with this status are returned.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The maximum number of requests per page.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The requested page of results.</returns>
    Task<PagedResult<VolunteerRequestResponse>> GetPagedAsync(
        string? status, int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Approves or rejects a pending volunteer request. Approving a request
    /// also creates a new active volunteer roster entry linked back to it.
    /// </summary>
    /// <param name="id">The request's id.</param>
    /// <param name="request">The review decision and optional note.</param>
    /// <param name="reviewingAdminId">The id of the admin performing the review.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The updated request, or <see langword="null"/> if it does not exist.</returns>
    /// <exception cref="InvalidOperationException">The request has already been reviewed.</exception>
    Task<VolunteerRequestResponse?> ReviewAsync(
        long id, VolunteerRequestReviewRequest request, long reviewingAdminId, CancellationToken cancellationToken);
}
