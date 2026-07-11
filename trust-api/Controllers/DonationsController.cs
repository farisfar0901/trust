using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trust.Api.Data;
using Trust.Api.Domain;

namespace Trust.Api.Controllers;

[ApiController]
[Route("api/donations")]
public sealed class DonationsController(TrustDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DonationRecord>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await dbContext.DonationRecords
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<DonationRecord>> Create(DonationRecord request, CancellationToken cancellationToken)
    {
        request.CreatedAt = DateTime.UtcNow;
        request.Status = string.IsNullOrWhiteSpace(request.Status) ? "Received" : request.Status;

        dbContext.DonationRecords.Add(request);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(request);
    }
}