using Magnus.API.DTOs.AuditLogs;
using Magnus.API.Helpers;

namespace Magnus.API.Services.Interfaces;

public interface IAuditLogService
{
    Task<PagedResult<AuditLogReadDto>> GetPagedAsync(AuditLogQueryDto query, CancellationToken cancellationToken = default);
    Task<AuditLogReadDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
