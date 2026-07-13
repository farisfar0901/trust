using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Configurations;

public sealed class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
{
    public void Configure(EntityTypeBuilder<Volunteer> builder)
    {
        builder.ToTable("volunteers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.VolunteerRequestId).HasColumnName("volunteer_request_id");
        builder.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(150).IsRequired();
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(20).IsRequired();
        builder.Property(x => x.City).HasColumnName("city").HasMaxLength(100);
        builder.Property(x => x.AreaOfWork).HasColumnName("area_of_work").HasMaxLength(150);
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired().HasDefaultValue("Active");
        builder.Property(x => x.JoinedDate).HasColumnName("joined_date").HasColumnType("date")
            .IsRequired().HasDefaultValueSql("CURRENT_DATE");
        builder.Property(x => x.CreatedByAdminId).HasColumnName("created_by_admin_id");
        builder.Property(x => x.UpdatedByAdminId).HasColumnName("updated_by_admin_id");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamptz");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();

        builder.ToTable(t => t.HasCheckConstraint("chk_volunteers_status", "status IN ('Active', 'Inactive')"));
        builder.ToTable(t => t.HasCheckConstraint("chk_volunteers_email_format", "email ~* '^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$'"));

        builder.HasIndex(x => x.Email).HasDatabaseName("ux_volunteers_email").IsUnique().HasFilter("deleted_at IS NULL");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_volunteers_status").HasFilter("deleted_at IS NULL");
        // Names the unique index EF creates automatically for the one-to-one
        // relationship's FK, so it matches the schema exactly.
        builder.HasIndex(x => x.VolunteerRequestId).HasDatabaseName("uq_volunteers_volunteer_request_id").IsUnique();

        // Intentionally a plain (non-partial) UNIQUE, enforced via the one-to-one
        // relationship below: a request may be promoted to a volunteer at most
        // once, ever — soft-deleting the volunteer must not allow the same
        // request to be "re-promoted" into a second volunteer.
        builder.HasOne(x => x.VolunteerRequest)
            .WithOne(x => x.PromotedVolunteer)
            .HasForeignKey<Volunteer>(x => x.VolunteerRequestId)
            .HasConstraintName("fk_volunteers_request")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.CreatedByAdmin)
            .WithMany()
            .HasForeignKey(x => x.CreatedByAdminId)
            .HasConstraintName("fk_volunteers_created_by")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.UpdatedByAdmin)
            .WithMany()
            .HasForeignKey(x => x.UpdatedByAdminId)
            .HasConstraintName("fk_volunteers_updated_by")
            .OnDelete(DeleteBehavior.SetNull);
    }
}
