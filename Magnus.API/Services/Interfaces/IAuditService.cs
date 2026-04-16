namespace Magnus.API.Services.Interfaces;

public interface IAuditService
{
    Task LogAsync(string action, string entityName, string entityId, string? changes = null, string? userId = null, CancellationToken cancellationToken = default);
}
