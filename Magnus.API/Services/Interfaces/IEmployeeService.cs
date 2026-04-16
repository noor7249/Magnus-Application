using Magnus.API.DTOs.Common;
using Magnus.API.DTOs;
using Magnus.API.Helpers;

namespace Magnus.API.Services.Interfaces;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeReadDto>> GetPagedAsync(PagedQueryDto query, CancellationToken cancellationToken = default);
    Task<EmployeeReadDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EmployeeReadDto> CreateAsync(EmployeeCreateDto request, CancellationToken cancellationToken = default);
    Task<EmployeeReadDto> UpdateAsync(int id, EmployeeUpdateDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
