using Magnus.API.DTOs.Common;
using Magnus.API.DTOs.Designations;
using Magnus.API.Helpers;

namespace Magnus.API.Services.Interfaces;

public interface IDesignationService
{
    Task<PagedResult<DesignationReadDto>> GetPagedAsync(PagedQueryDto query, CancellationToken cancellationToken = default);
    Task<DesignationReadDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DesignationReadDto> CreateAsync(DesignationCreateDto request, CancellationToken cancellationToken = default);
    Task<DesignationReadDto> UpdateAsync(int id, DesignationUpdateDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
