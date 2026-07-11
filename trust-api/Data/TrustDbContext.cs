using Microsoft.EntityFrameworkCore;
using Trust.Api.Domain;

namespace Trust.Api.Data;

public sealed class TrustDbContext(DbContextOptions<TrustDbContext> options) : DbContext(options)
{
    public DbSet<EventItem> Events => Set<EventItem>();
    public DbSet<GalleryPhoto> GalleryPhotos => Set<GalleryPhoto>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<VolunteerApplication> VolunteerApplications => Set<VolunteerApplication>();
    public DbSet<DonationRecord> DonationRecords => Set<DonationRecord>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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