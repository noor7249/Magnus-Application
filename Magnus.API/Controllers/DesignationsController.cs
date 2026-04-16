using Magnus.API.DTOs.Common;
using Magnus.API.DTOs.Designations;
using Magnus.API.Helpers;
using Magnus.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Magnus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DesignationsController : ControllerBase
{
    private readonly IDesignationService _designationService;

    public DesignationsController(IDesignationService designationService)
    {
        _designationService = designationService;
    }

    [HttpGet]
    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Manager},{RoleConstants.Employee}")]
    public async Task<ActionResult<ApiResponse<PagedResult<DesignationReadDto>>>> GetAll([FromQuery] PagedQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _designationService.GetPagedAsync(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<DesignationReadDto>>.SuccessResponse(result, "Designations retrieved successfully."));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Manager},{RoleConstants.Employee}")]
    public async Task<ActionResult<ApiResponse<DesignationReadDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _designationService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<DesignationReadDto>.SuccessResponse(result, "Designation retrieved successfully."));
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<ActionResult<ApiResponse<DesignationReadDto>>> Create([FromBody] DesignationCreateDto request, CancellationToken cancellationToken)
    {
        var result = await _designationService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<DesignationReadDto>.SuccessResponse(result, "Designation created successfully."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Manager}")]
    public async Task<ActionResult<ApiResponse<DesignationReadDto>>> Update(int id, [FromBody] DesignationUpdateDto request, CancellationToken cancellationToken)
    {
        var result = await _designationService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<DesignationReadDto>.SuccessResponse(result, "Designation updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        await _designationService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Designation deleted successfully."));
    }
}
