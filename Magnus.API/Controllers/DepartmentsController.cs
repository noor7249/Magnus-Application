using Magnus.API.DTOs.Common;
using Magnus.API.DTOs.Departments;
using Magnus.API.Helpers;
using Magnus.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Magnus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Manager},{RoleConstants.Employee}")]
    public async Task<ActionResult<ApiResponse<PagedResult<DepartmentReadDto>>>> GetAll([FromQuery] PagedQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _departmentService.GetPagedAsync(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<DepartmentReadDto>>.SuccessResponse(result, "Departments retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Manager},{RoleConstants.Employee}")]
    public async Task<ActionResult<ApiResponse<DepartmentReadDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _departmentService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<DepartmentReadDto>.SuccessResponse(result, "Department retrieved successfully."));
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<ActionResult<ApiResponse<DepartmentReadDto>>> Create([FromBody] DepartmentCreateDto request, CancellationToken cancellationToken)
    {
        var result = await _departmentService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<DepartmentReadDto>.SuccessResponse(result, "Department created successfully."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Manager}")]
    public async Task<ActionResult<ApiResponse<DepartmentReadDto>>> Update(int id, [FromBody] DepartmentUpdateDto request, CancellationToken cancellationToken)
    {
        var result = await _departmentService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<DepartmentReadDto>.SuccessResponse(result, "Department updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        await _departmentService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Department deleted successfully."));
    }
}
