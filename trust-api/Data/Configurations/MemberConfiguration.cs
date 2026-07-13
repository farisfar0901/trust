using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Configurations;

public sealed class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("members");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(150).IsRequired();
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(20).IsRequired();
        builder.Property(x => x.Address).HasColumnName("address");
        builder.Property(x => x.MembershipType).HasColumnName("membership_type").HasMaxLength(30).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired().HasDefaultValue("Active");
        builder.Property(x => x.JoinedDate).HasColumnName("joined_date").HasColumnType("date")
            .IsRequired().HasDefaultValueSql("CURRENT_DATE");
        builder.Property(x => x.PhotoUrl).HasColumnName("photo_url").HasMaxLength(500);
        builder.Property(x => x.CreatedByAdminId).HasColumnName("created_by_admin_id");
        builder.Property(x => x.UpdatedByAdminId).HasColumnName("updated_by_admin_id");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamptz");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();

        builder.ToTable(t => t.HasCheckConstraint(
            "chk_members_membership_type", "membership_type IN ('General', 'Lifetime', 'Honorary', 'Patron')"));
        builder.ToTable(t => t.HasCheckConstraint("chk_members_status", "status IN ('Active', 'Inactive')"));
        builder.ToTable(t => t.HasCheckConstraint("chk_members_email_format", "email ~* '^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$'"));

        builder.HasIndex(x => x.Email).HasDatabaseName("ux_members_email").IsUnique().HasFilter("deleted_at IS NULL");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_members_status").HasFilter("deleted_at IS NULL");
        builder.HasIndex(x => x.MembershipType).HasDatabaseName("ix_members_membership_type").HasFilter("deleted_at IS NULL");

        builder.HasOne(x => x.CreatedByAdmin)
            .WithMany()
            .HasForeignKey(x => x.CreatedByAdminId)
            .HasConstraintName("fk_members_created_by")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.UpdatedByAdmin)
            .WithMany()
            .HasForeignKey(x => x.UpdatedByAdminId)
            .HasConstraintName("fk_members_updated_by")
            .OnDelete(DeleteBehavior.SetNull);
    }
}
