using Trust.Api.Domain.Admin;

namespace Trust.Api.Application.Repositories;

/// <summary>
/// Data access for <see cref="Member"/> records.
/// </summary>
public interface IMemberRepository
{
    /// <summary>
    /// Finds a member by id, excluding soft-deleted rows.
    /// </summary>
    /// <param name="id">The member's id.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching member, or <see langword="null"/> if none exists.</returns>
    Task<Member?> GetByIdAsync(long id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves one page of members, optionally filtered by status and by a
    /// case-insensitive search over the member's name, ordered for display.
    /// </summary>
    /// <param name="status">When provided, only members with this status (<c>Active</c> or <c>Inactive</c>) are returned.</param>
    /// <param name="searchText">When provided, only members whose full name contains this text (case-insensitive) is returned.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The maximum number of items per page.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The matching members for the page, and the total count across all pages.</returns>
    Task<(IReadOnlyList<Member> Items, int TotalCount)> GetPagedAsync(
        string? status,
        string? searchText,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Checks whether an (non-soft-deleted) member already uses the given
    /// email address, case-insensitively.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="excludingId">When provided, a member id to exclude from the check (used when updating a member's own record).</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns><see langword="true"/> if another member already uses this email; otherwise <see langword="false"/>.</returns>
    Task<bool> ExistsWithEmailAsync(string email, long? excludingId, CancellationToken cancellationToken);

    /// <summary>
    /// Inserts a new member.
    /// </summary>
    /// <param name="member">The member to insert.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task AddAsync(Member member, CancellationToken cancellationToken);

    /// <summary>
    /// Persists changes made to an existing member (including edits and soft
    /// deletes, which the service expresses by setting <c>DeletedAt</c>).
    /// </summary>
    /// <param name="member">The member, with its properties already updated by the caller.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task UpdateAsync(Member member, CancellationToken cancellationToken);
}
