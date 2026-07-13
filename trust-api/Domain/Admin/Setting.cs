namespace Trust.Api.Domain.Admin;

public sealed class Setting
{
    public long Id { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string? SettingValue { get; set; }
    public string? SettingGroup { get; set; }
    public bool IsSecret { get; set; }
    public long? UpdatedByAdminId { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public AdminUser? UpdatedByAdmin { get; set; }
}
