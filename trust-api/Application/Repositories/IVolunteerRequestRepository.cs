using Trust.Api.Domain.Admin;

namespace Trust.Api.Application.Repositories;

/// <summary>
/// Data access for <see cref="VolunteerRequest"/> records. Contains only
/// reads and writes — approval/rejection rules live in the service layer.
/// </summary>
public interface IVolunteerRequestRepository
{
    /// <summary>
    /// Finds a volunteer request by id, excluding soft-deleted rows.
    /// </summary>
    /// <param name="id">The request's id.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching request, or <see langword="null"/> if none exists.</returns>
    Task<VolunteerRequest?> GetByIdAsync(long id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves one page of volunteer requests, optionally filtered by status,
    /// newest first.
    /// </summary>
    /// <param name="status">When provided, only requests with this status are returned.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The maximum number of requests per page.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching requests for the page, and the total count across all pages.</returns>
    Task<(IReadOnlyList<VolunteerRequest> Items, int TotalCount)> GetPagedAsync(
        string? status, int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Inserts a new volunteer request submitted by a member of the public.
    /// </summary>
    /// <param name="volunteerRequest">The request to insert.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task AddAsync(VolunteerRequest volunteerRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Persists changes made to an existing volunteer request (for example
    /// its review status, note, and reviewer).
    /// </summary>
    /// <param name="volunteerRequest">The request, with its properties already updated by the caller.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task UpdateAsync(VolunteerRequest volunteerRequest, CancellationToken cancellationToken);
}
