using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Configurations;

public sealed class VolunteerRequestConfiguration : IEntityTypeConfiguration<VolunteerRequest>
{
    public void Configure(EntityTypeBuilder<VolunteerRequest> builder)
    {
        builder.ToTable("volunteer_requests");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(150).IsRequired();
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(20).IsRequired();
        builder.Property(x => x.City).HasColumnName("city").HasMaxLength(100);
        builder.Property(x => x.PreferredArea).HasColumnName("preferred_area").HasMaxLength(150);
        builder.Property(x => x.Message).HasColumnName("message");
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired().HasDefaultValue("Pending");
        builder.Property(x => x.ReviewNote).HasColumnName("review_note");
        builder.Property(x => x.ReviewedByAdminId).HasColumnName("reviewed_by_admin_id");
        builder.Property(x => x.ReviewedAt).HasColumnName("reviewed_at").HasColumnType("timestamptz");
        builder.Property(x => x.IpAddress).HasColumnName("ip_address").HasColumnType("inet");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamptz");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();

        builder.ToTable(t => t.HasCheckConstraint(
            "chk_volunteer_requests_status", "status IN ('Pending', 'Approved', 'Rejected')"));
        builder.ToTable(t => t.HasCheckConstraint(
            "chk_volunteer_requests_email_format", "email ~* '^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$'"));

        builder.HasIndex(x => x.Status).HasDatabaseName("ix_volunteer_requests_status").HasFilter("deleted_at IS NULL");
        builder.HasIndex(x => x.CreatedAt).HasDatabaseName("ix_volunteer_requests_created_at").HasFilter("deleted_at IS NULL");
        // Deliberately not unique: the same person may submit more than one request over time.
        builder.HasIndex(x => x.Email).HasDatabaseName("ix_volunteer_requests_email").HasFilter("deleted_at IS NULL");

        builder.HasOne(x => x.ReviewedByAdmin)
            .WithMany()
            .HasForeignKey(x => x.ReviewedByAdminId)
            .HasConstraintName("fk_volunteer_requests_reviewed_by")
            .OnDelete(DeleteBehavior.SetNull);
    }
}
