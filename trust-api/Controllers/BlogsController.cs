using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trust.Api.Data;
using Trust.Api.Domain;
using Trust.Api.Infrastructure;

namespace Trust.Api.Controllers;

[ApiController]
[Route("api/blogs")]
public sealed class BlogsController(TrustDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BlogPost>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await dbContext.BlogPosts
            .OrderByDescending(item => item.PublishedAt)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<BlogPost>> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var item = await dbContext.BlogPosts.FirstOrDefaultAsync(item => item.Slug == slug, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<BlogPost>> Create(BlogPost request, CancellationToken cancellationToken)
    {
        request.Slug = string.IsNullOrWhiteSpace(request.Slug)
            ? SlugHelper.ToSlug(request.Title)
            : SlugHelper.ToSlug(request.Slug);
        request.PublishedAt = request.PublishedAt == default ? DateTime.UtcNow : request.PublishedAt;
        request.CreatedAt = DateTime.UtcNow;

        dbContext.BlogPosts.Add(request);
        await dbContext.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetBySlug), new { slug = request.Slug }, request);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, BlogPost request, CancellationToken cancellationToken)
    {
        var existing = await dbContext.BlogPosts.FindAsync([id], cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Title = request.Title;
        existing.Slug = string.IsNullOrWhiteSpace(request.Slug)
            ? SlugHelper.ToSlug(request.Title)
            : SlugHelper.ToSlug(request.Slug);
        existing.Category = request.Category;
        existing.Excerpt = request.Excerpt;
        existing.Content = request.Content;
        existing.PublishedAt = request.PublishedAt == default ? existing.PublishedAt : request.PublishedAt;
        existing.IsPublished = request.IsPublished;

        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}