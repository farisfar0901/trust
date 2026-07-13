using System.Net;

namespace Trust.Api.Domain.Admin;

public sealed class AuditLog
{
    public long Id { get; set; }
    public long? AdminId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public long? EntityId { get; set; }
    public string? Metadata { get; set; }
    public IPAddress? IpAddress { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public AdminUser? Admin { get; set; }
}
