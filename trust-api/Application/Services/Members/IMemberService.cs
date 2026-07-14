using Trust.Api.Application.Common;
using Trust.Api.Application.Services.Members.DTOs;

namespace Trust.Api.Application.Services.Members;

/// <summary>
/// Business logic for creating, reading, updating, and soft-deleting members.
/// </summary>
public interface IMemberService
{
    /// <summary>
    /// Adds a new member, validating that the email address is not already
    /// in use by another member.
    /// </summary>
    /// <param name="request">The member's details.</param>
    /// <param name="creatingAdminId">The id of the admin creating the member.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The newly created member.</returns>
    /// <exception cref="ArgumentException"><paramref name="request"/> uses an email address already in use.</exception>
    Task<MemberResponse> CreateAsync(
        MemberCreateRequest request, long creatingAdminId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a single member by id.
    /// </summary>
    /// <param name="id">The member's id.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching member, or <see langword="null"/> if none exists.</returns>
    Task<MemberResponse?> GetByIdAsync(long id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves one page of members, optionally filtered by status and by a
    /// search over the member's name.
    /// </summary>
    /// <param name="status">When provided, only members with this status (<c>Active</c> or <c>Inactive</c>) are returned.</param>
    /// <param name="searchText">When provided, only members whose name contains this text are returned.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The maximum number of items per page.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The requested page of results.</returns>
    Task<PagedResult<MemberResponse>> GetPagedAsync(
        string? status,
        string? searchText,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Replaces an existing member's details.
    /// </summary>
    /// <param name="id">The member's id.</param>
    /// <param name="request">The member's new details.</param>
    /// <param name="updatingAdminId">The id of the admin making the change.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The updated member, or <see langword="null"/> if it does not exist.</returns>
    /// <exception cref="ArgumentException"><paramref name="request"/> uses an email address already in use by another member.</exception>
    Task<MemberResponse?> UpdateAsync(
        long id, MemberUpdateRequest request, long updatingAdminId, CancellationToken cancellationToken);

    /// <summary>
    /// Soft-deletes a member so it no longer appears in normal listings.
    /// </summary>
    /// <param name="id">The member's id.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns><see langword="true"/> if the member was found and deleted; otherwise <see langword="false"/>.</returns>
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
}
