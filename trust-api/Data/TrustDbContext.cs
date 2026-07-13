using Microsoft.EntityFrameworkCore;
using Trust.Api.Domain;
using Admin = Trust.Api.Domain.Admin;

namespace Trust.Api.Data;

public sealed class TrustDbContext(DbContextOptions<TrustDbContext> options) : DbContext(options)
{
    public DbSet<EventItem> Events => Set<EventItem>();
    public DbSet<GalleryPhoto> GalleryPhotos => Set<GalleryPhoto>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<VolunteerApplication> VolunteerApplications => Set<VolunteerApplication>();
    public DbSet<DonationRecord> DonationRecords => Set<DonationRecord>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    // Admin domain layer (docs/DATABASE_DESIGN.md, database/schema.sql).
    // "Admin" prefix only where the name would otherwise collide with the
    // legacy DbSets above (Events, ContactMessages); the entity classes
    // themselves are named cleanly (Event, ContactMessage) in Domain.Admin.
    public DbSet<Admin.AdminUser> AdminUsers => Set<Admin.AdminUser>();
    public DbSet<Admin.AdminAuthToken> AdminAuthTokens => Set<Admin.AdminAuthToken>();
    public DbSet<Admin.Member> Members => Set<Admin.Member>();
    public DbSet<Admin.VolunteerRequest> VolunteerRequests => Set<Admin.VolunteerRequest>();
    public DbSet<Admin.Volunteer> Volunteers => Set<Admin.Volunteer>();
    public DbSet<Admin.Event> AdminEvents => Set<Admin.Event>();
    public DbSet<Admin.GalleryMedia> GalleryMedia => Set<Admin.GalleryMedia>();
    public DbSet<Admin.ContactMessage> AdminContactMessages => Set<Admin.ContactMessage>();
    public DbSet<Admin.Announcement> Announcements => Set<Admin.Announcement>();
    public DbSet<Admin.Testimonial> Testimonials => Set<Admin.Testimonial>();
    public DbSet<Admin.Setting> Settings => Set<Admin.Setting>();
    public DbSet<Admin.AuditLog> AuditLogs => Set<Admin.AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrustDbContext).Assembly);

        modelBuilder.Entity<EventItem>(entity =>
        {
            entity.HasIndex(item => item.Slug).IsUnique();
            entity.Property(item => item.Title).HasMaxLength(200).IsRequired();
            entity.Property(item => item.Slug).HasMaxLength(220).IsRequired();
            entity.Property(item => item.Category).HasMaxLength(120).IsRequired();
            entity.Property(item => item.Location).HasMaxLength(180).IsRequired();
        });

        modelBuilder.Entity<GalleryPhoto>(entity =>
        {
            entity.Property(item => item.Title).HasMaxLength(200).IsRequired();
            entity.Property(item => item.MediaType).HasMaxLength(50).IsRequired();
            entity.Property(item => item.Caption).HasMaxLength(500).IsRequired();
        });

        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.HasIndex(item => item.Slug).IsUnique();
            entity.Property(item => item.Title).HasMaxLength(220).IsRequired();
            entity.Property(item => item.Slug).HasMaxLength(240).IsRequired();
            entity.Property(item => item.Category).HasMaxLength(120).IsRequired();
        });

        modelBuilder.Entity<VolunteerApplication>(entity =>
        {
            entity.Property(item => item.FullName).HasMaxLength(180).IsRequired();
            entity.Property(item => item.Email).HasMaxLength(180).IsRequired();
            entity.Property(item => item.Phone).HasMaxLength(40).IsRequired();
            entity.Property(item => item.City).HasMaxLength(120).IsRequired();
            entity.Property(item => item.PreferredArea).HasMaxLength(120).IsRequired();
        });

        modelBuilder.Entity<DonationRecord>(entity =>
        {
            entity.Property(item => item.DonorName).HasMaxLength(180).IsRequired();
            entity.Property(item => item.Email).HasMaxLength(180).IsRequired();
            entity.Property(item => item.PaymentMethod).HasMaxLength(80).IsRequired();
            entity.Property(item => item.ReferenceNumber).HasMaxLength(100).IsRequired();
            entity.Property(item => item.Amount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.Property(item => item.Name).HasMaxLength(180).IsRequired();
            entity.Property(item => item.Email).HasMaxLength(180).IsRequired();
            entity.Property(item => item.Subject).HasMaxLength(200).IsRequired();
        });
    }
}