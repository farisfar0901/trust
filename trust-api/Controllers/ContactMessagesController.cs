using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trust.Api.Data;
using Trust.Api.Domain;

namespace Trust.Api.Controllers;

[ApiController]
[Route("api/contact-messages")]
public sealed class ContactMessagesController(TrustDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContactMessage>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await dbContext.ContactMessages
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<ContactMessage>> Create(ContactMessage request, CancellationToken cancellationToken)
    {
        request.CreatedAt = DateTime.UtcNow;
        dbContext.ContactMessages.Add(request);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(request);
    }
}