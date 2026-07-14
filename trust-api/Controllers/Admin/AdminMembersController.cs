using Microsoft.AspNetCore.Mvc;
using Trust.Api.Application.Common;
using Trust.Api.Application.Services.Members;
using Trust.Api.Application.Services.Members.DTOs;
using Trust.Api.Authorization;

namespace Trust.Api.Controllers.Admin;

/// <summary>
/// HTTP endpoints for admins to manage members. This controller only
/// translates HTTP requests into calls on <see cref="IMemberService"/> and
/// maps the results to HTTP responses — all email-uniqueness validation and
/// soft-delete logic lives in the service layer.
/// </summary>
[ApiController]
[Route("api/admin/members")]
[RequireAdminRole("SuperAdmin", "Admin")]
public sealed class AdminMembersController : ControllerBase
{
    /// <summary>
    /// Largest page size a caller may request, to avoid accidentally loading
    /// the entire table in one response.
    /// </summary>
    private const int MaxPageSize = 100;

    private readonly IMemberService _memberService;

    /// <summary>
    /// Creates the controller.
    /// </summary>
    /// <param name="memberService">The service that reads, writes, and soft-deletes members.</param>
    public AdminMembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    /// <summary>
    /// Retrieves one page of members, optionally filtered by status and by a
    /// name search.
    /// </summary>
    /// <param name="status">When provided, only members with this status (<c>Active</c> or <c>Inactive</c>) are returned.</param>
    /// <param name="search">When provided, only members whose name contains this text (case-insensitive) are returned.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The maximum number of items per page (1-100). Defaults to 20.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<MemberResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] string? status = null,
        [FromQuery] string? search = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var result = await _memberService.GetPagedAsync(status, search, pageNumber, pageSize, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single member by id.
    /// </summary>
    /// <param name="id">The member's id.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(MemberResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var member = await _memberService.GetByIdAsync(id, cancellationToken);
        return member is null ? NotFound() : Ok(member);
    }

    /// <summary>
    /// Adds a new member.
    /// </summary>
    /// <param name="request">The member's details.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpPost]
    [ProducesResponseType(typeof(MemberResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] MemberCreateRequest request, CancellationToken cancellationToken)
    {
        var creatingAdminId = User.GetAdminId();

        try
        {
            var created = await _memberService.CreateAsync(request, creatingAdminId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            // The service throws this when the request reuses an email
            // address already in use — a client input problem, not a server error.
            return ValidationProblem(ex.Message);
        }
    }

    /// <summary>
    /// Replaces an existing member's details.
    /// </summary>
    /// <param name="id">The member's id.</param>
    /// <param name="request">The member's new details.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(MemberResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long id, [FromBody] MemberUpdateRequest request, CancellationToken cancellationToken)
    {
        var updatingAdminId = User.GetAdminId();

        try
        {
            var updated = await _memberService.UpdateAsync(id, request, updatingAdminId, cancellationToken);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return ValidationProblem(ex.Message);
        }
    }

    /// <summary>
    /// Soft-deletes a member so it no longer appears in normal listings. The
    /// underlying row is kept (<c>deleted_at</c> is set), never removed.
    /// </summary>
    /// <param name="id">The member's id.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var deleted = await _memberService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
