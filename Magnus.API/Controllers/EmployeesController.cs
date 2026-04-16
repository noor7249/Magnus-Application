using Magnus.API.DTOs;
using Magnus.API.DTOs.Common;
using Magnus.API.Helpers;
using Magnus.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Magnus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Manager}")]
    public async Task<ActionResult<ApiResponse<PagedResult<EmployeeReadDto>>>> GetAll([FromQuery] PagedQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetPagedAsync(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<EmployeeReadDto>>.SuccessResponse(result, "Employees retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Manager}")]
    public async Task<ActionResult<ApiResponse<EmployeeReadDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<EmployeeReadDto>.SuccessResponse(result, "Employee retrieved successfully."));
    }

    [HttpPost]
    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Manager}")]
    public async Task<ActionResult<ApiResponse<EmployeeReadDto>>> Create([FromBody] EmployeeCreateDto request, CancellationToken cancellationToken)
    {
        var result = await _employeeService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<EmployeeReadDto>.SuccessResponse(result, "Employee created successfully."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Manager}")]
    public async Task<ActionResult<ApiResponse<EmployeeReadDto>>> Update(int id, [FromBody] EmployeeUpdateDto request, CancellationToken cancellationToken)
    {
        var result = await _employeeService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<EmployeeReadDto>.SuccessResponse(result, "Employee updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        await _employeeService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Employee deleted successfully."));
    }
}
