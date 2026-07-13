using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trust.Api.Domain.Admin;

namespace Trust.Api.Data.Configurations;

public sealed class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ToTable("settings");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.SettingKey).HasColumnName("setting_key").HasMaxLength(100).IsRequired();
        builder.Property(x => x.SettingValue).HasColumnName("setting_value");
        builder.Property(x => x.SettingGroup).HasColumnName("setting_group").HasMaxLength(50);
        builder.Property(x => x.IsSecret).HasColumnName("is_secret").IsRequired().HasDefaultValue(false);
        builder.Property(x => x.UpdatedByAdminId).HasColumnName("updated_by_admin_id");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamptz")
            .IsRequired().HasDefaultValueSql("now()").ValueGeneratedOnAdd();

        // No deleted_at: config rows are upserted/overwritten, not "recovered".
        builder.HasIndex(x => x.SettingKey).HasDatabaseName("uq_settings_setting_key").IsUnique();
        builder.HasIndex(x => x.SettingGroup).HasDatabaseName("ix_settings_group");

        builder.HasOne(x => x.UpdatedByAdmin)
            .WithMany()
            .HasForeignKey(x => x.UpdatedByAdminId)
            .HasConstraintName("fk_settings_updated_by")
            .OnDelete(DeleteBehavior.SetNull);
    }
}
