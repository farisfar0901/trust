using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trust.Api.Data;
using Trust.Api.Domain;

namespace Trust.Api.Controllers;

[ApiController]
[Route("api/gallery")]
public sealed class GalleryController(TrustDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GalleryPhoto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await dbContext.GalleryPhotos
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GalleryPhoto>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await dbContext.GalleryPhotos.FindAsync([id], cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<GalleryPhoto>> Create(GalleryPhoto request, CancellationToken cancellationToken)
    {
        request.CreatedAt = DateTime.UtcNow;
        dbContext.GalleryPhotos.Add(request);
        await dbContext.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, GalleryPhoto request, CancellationToken cancellationToken)
    {
        var existing = await dbContext.GalleryPhotos.FindAsync([id], cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        existing.EventItemId = request.EventItemId;
        existing.Title = request.Title;
        existing.Caption = request.Caption;
        existing.ImageUrl = request.ImageUrl;
        existing.MediaType = request.MediaType;
        existing.Year = request.Year;
        existing.TakenAt = request.TakenAt;
        existing.IsPublished = request.IsPublished;

        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var existing = await dbContext.GalleryPhotos.FindAsync([id], cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        dbContext.GalleryPhotos.Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}