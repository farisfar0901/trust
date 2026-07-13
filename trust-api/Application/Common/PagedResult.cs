namespace Trust.Api.Application.Common;

/// <summary>
/// A single page of results from a larger, filterable list, along with
/// enough information for the caller to render pagination controls.
/// </summary>
/// <typeparam name="T">The type of item in the page.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>
    /// The items on this page.
    /// </summary>
    public required IReadOnlyList<T> Items { get; init; }

    /// <summary>
    /// The total number of items across all pages, ignoring paging.
    /// </summary>
    public required int TotalCount { get; init; }

    /// <summary>
    /// The 1-based page number this result represents.
    /// </summary>
    public required int PageNumber { get; init; }

    /// <summary>
    /// The maximum number of items per page.
    /// </summary>
    public required int PageSize { get; init; }

    /// <summary>
    /// The total number of pages, given <see cref="TotalCount"/> and <see cref="PageSize"/>.
    /// </summary>
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
