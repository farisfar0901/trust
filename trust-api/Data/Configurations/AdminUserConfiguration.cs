using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Configurations;

public sealed class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> builder)
    {
        builder.ToTable("admin_users");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(150).IsRequired();
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
        builder.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
        builder.Property(x => x.Role).HasColumnName("role").HasMaxLength(30).IsRequired().HasDefaultValue("Editor");
        builder.Property(x => x.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);
        builder.Property(x => x.MfaEnabled).HasColumnName("mfa_enabled").IsRequired().HasDefaultValue(false);
        builder.Property(x => x.MfaSecret).HasColumnName("mfa_secret").HasMaxLength(255);
        builder.Property(x => x.FailedLoginAttempts).HasColumnName("failed_login_attempts").IsRequired().HasDefaultValue((short)0);
        builder.Property(x => x.LockedUntil).HasColumnName("locked_until").HasColumnType("timestamptz");
        builder.Property(x => x.PasswordChangedAt).HasColumnName("password_changed_at").HasColumnType("timestamptz");
        builder.Property(x => x.LastLoginAt).HasColumnName("last_login_at").HasColumnType("timestamptz");
        builder.Property(x => x.LastLoginIp).HasColumnName("last_login_ip").HasColumnType("inet");
        builder.Property(x => x.CreatedByAdminId).HasColumnName("created_by_admin_id");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamptz");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();

        builder.ToTable(t => t.HasCheckConstraint("chk_admin_users_role", "role IN ('SuperAdmin', 'Admin', 'Editor')"));
        builder.ToTable(t => t.HasCheckConstraint("chk_admin_users_email_format", "email ~* '^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$'"));

        // Partial: only one active (non-deleted) account may hold a given email.
        builder.HasIndex(x => x.Email).HasDatabaseName("ux_admin_users_email").IsUnique()
            .HasFilter("deleted_at IS NULL");
        builder.HasIndex(x => x.IsActive).HasDatabaseName("ix_admin_users_is_active")
            .HasFilter("deleted_at IS NULL");
        // Not partial by design: this index exists to find soft-deleted rows.
        builder.HasIndex(x => x.DeletedAt).HasDatabaseName("ix_admin_users_deleted_at");

        builder.HasOne(x => x.CreatedByAdmin)
            .WithMany(x => x.ProvisionedAdmins)
            .HasForeignKey(x => x.CreatedByAdminId)
            .HasConstraintName("fk_admin_users_created_by")
            .OnDelete(DeleteBehavior.SetNull);
    }
}
