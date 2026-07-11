using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trust.Api.Data;
using Trust.Api.Domain;
using Trust.Api.Infrastructure;

namespace Trust.Api.Controllers;

[ApiController]
[Route("api/events")]
public sealed class EventsController(TrustDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventItem>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await dbContext.Events
            .OrderByDescending(item => item.EventDate)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<EventItem>>> GetUpcoming(CancellationToken cancellationToken)
    {
        var items = await dbContext.Events
            .Where(item => item.IsUpcoming)
            .OrderBy(item => item.EventDate)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventItem>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await dbContext.Events.FindAsync([id], cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<EventItem>> Create(EventItem request, CancellationToken cancellationToken)
    {
        request.Slug = string.IsNullOrWhiteSpace(request.Slug)
            ? SlugHelper.ToSlug(request.Title)
            : SlugHelper.ToSlug(request.Slug);
        request.CreatedAt = DateTime.UtcNow;

        dbContext.Events.Add(request);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EventItem request, CancellationToken cancellationToken)
    {
        var existing = await dbContext.Events.FindAsync([id], cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Title = request.Title;
        existing.Slug = string.IsNullOrWhiteSpace(request.Slug)
            ? SlugHelper.ToSlug(request.Title)
            : SlugHelper.ToSlug(request.Slug);
        existing.Category = request.Category;
        existing.Description = request.Description;
        existing.Location = request.Location;
        existing.EventDate = request.EventDate;
        existing.IsUpcoming = request.IsUpcoming;
        existing.IsPublished = request.IsPublished;
        existing.CoverImageUrl = request.CoverImageUrl;

        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existing = await dbContext.Events.FindAsync([id], cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        dbContext.Events.Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}