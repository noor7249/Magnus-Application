using Magnus.API.DTOs.Common;
using Magnus.API.DTOs.Departments;
using Magnus.API.Helpers;

namespace Magnus.API.Services.Interfaces;

public interface IDepartmentService
{
    Task<PagedResult<DepartmentReadDto>> GetPagedAsync(PagedQueryDto query, CancellationToken cancellationToken = default);
    Task<DepartmentReadDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DepartmentReadDto> CreateAsync(DepartmentCreateDto request, CancellationToken cancellationToken = default);
    Task<DepartmentReadDto> UpdateAsync(int id, DepartmentUpdateDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
