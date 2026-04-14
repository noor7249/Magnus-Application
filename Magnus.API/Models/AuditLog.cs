using System.ComponentModel.DataAnnotations;

namespace Magnus.API.Models;

public class AuditLog
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string Action { get; set; } = string.Empty;

    [Required]
    public string EntityName { get; set; } = string.Empty;

    [Required]
    public string EntityId { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string? Changes { get; set; }
}
