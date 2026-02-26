using System;

namespace SmartHotelManagement.Model;

public class AuditLog
{
    public int AuditLogId { get; set; }
    public string AdminUserId { get; set; } = string.Empty;
    public string TargetUserId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
