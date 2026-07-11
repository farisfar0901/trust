using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trust.Api.Data;
using Trust.Api.Domain;

namespace Trust.Api.Controllers;

[ApiController]
[Route("api/volunteers")]
public sealed class VolunteersController(TrustDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VolunteerApplication>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await dbContext.VolunteerApplications
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<VolunteerApplication>> Create(VolunteerApplication request, CancellationToken cancellationToken)
    {
        request.CreatedAt = DateTime.UtcNow;
        request.Status = string.IsNullOrWhiteSpace(request.Status) ? "Pending" : request.Status;

        dbContext.VolunteerApplications.Add(request);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(request);
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status, CancellationToken cancellationToken)
    {
        var existing = await dbContext.VolunteerApplications.FindAsync([id], cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Status = status;
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}