using Magnus.API.DTOs.AuditLogs;
using Magnus.API.Helpers;
using Magnus.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Magnus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RoleConstants.Admin)]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogsController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<AuditLogReadDto>>>> GetAll([FromQuery] AuditLogQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _auditLogService.GetPagedAsync(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<AuditLogReadDto>>.SuccessResponse(result, "Audit logs retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<AuditLogReadDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _auditLogService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<AuditLogReadDto>.SuccessResponse(result, "Audit log retrieved successfully."));
    }
}
