using AutoMapper;
using AutoMapper.QueryableExtensions;
using Magnus.API.Data;
using Magnus.API.DTOs.AuditLogs;
using Magnus.API.Helpers;
using Magnus.API.Middleware;
using Magnus.API.Services.Interfaces;
using Magnus.API.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Magnus.API.Services.Implementations;

public class AuditLogService : IAuditLogService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AuditLogService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<AuditLogReadDto>> GetPagedAsync(AuditLogQueryDto query, CancellationToken cancellationToken = default)
    {
        var logs = _context.AuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.Trim().ToLower();
            logs = logs.Where(x => x.EntityId.ToLower().Contains(searchTerm) || x.UserId.ToLower().Contains(searchTerm) || (x.Changes != null && x.Changes.ToLower().Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(query.Action))
        {
            var action = query.Action.Trim().ToLower();
            logs = logs.Where(x => x.Action.ToLower() == action);
        }

        if (!string.IsNullOrWhiteSpace(query.EntityName))
        {
            var entityName = query.EntityName.Trim().ToLower();
            logs = logs.Where(x => x.EntityName.ToLower() == entityName);
        }

        logs = query.Descending ? logs.OrderByDescending(x => x.Timestamp) : logs.OrderBy(x => x.Timestamp);

        return await logs.ProjectTo<AuditLogReadDto>(_mapper.ConfigurationProvider).ToPagedResultAsync(query, cancellationToken);
    }

    public async Task<AuditLogReadDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var log = await _context.AuditLogs.AsNoTracking()
            .ProjectTo<AuditLogReadDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return log ?? throw new AppException($"Audit log with id {id} was not found.", HttpStatusCode.NotFound);
    }
}
