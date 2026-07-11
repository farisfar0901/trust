using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trust.Api.Data;

namespace Trust.Api.Controllers;

public sealed record AdminDashboardSummary(
    int Events,
    int UpcomingEvents,
    int GalleryPhotos,
    int BlogPosts,
    int VolunteerApplications,
    int PendingApplications,
    decimal TotalDonations);

[ApiController]
[Route("api/admin")]
public sealed class AdminController(TrustDbContext dbContext) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<ActionResult<AdminDashboardSummary>> GetDashboard(CancellationToken cancellationToken)
    {
        var eventsCount = await dbContext.Events.CountAsync(cancellationToken);
        var upcomingCount = await dbContext.Events.CountAsync(item => item.IsUpcoming, cancellationToken);
        var galleryCount = await dbContext.GalleryPhotos.CountAsync(cancellationToken);
        var blogCount = await dbContext.BlogPosts.CountAsync(cancellationToken);
        var applicationCount = await dbContext.VolunteerApplications.CountAsync(cancellationToken);
        var pendingApplications = await dbContext.VolunteerApplications.CountAsync(item => item.Status == "Pending", cancellationToken);
        var totalDonations = await dbContext.DonationRecords.SumAsync(item => (decimal?)item.Amount, cancellationToken) ?? 0m;

        return Ok(new AdminDashboardSummary(
            eventsCount,
            upcomingCount,
            galleryCount,
            blogCount,
            applicationCount,
            pendingApplications,
            totalDonations));
    }
}