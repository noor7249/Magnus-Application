using Magnus.API.DTOs.Common;
using System.ComponentModel.DataAnnotations;

namespace Magnus.API.DTOs.AuditLogs;

public class AuditLogQueryDto : PagedQueryDto
{
    [MaxLength(100)]
    public string? Action { get; set; }

    [MaxLength(150)]
    public string? EntityName { get; set; }
}
