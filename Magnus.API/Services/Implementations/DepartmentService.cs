using AutoMapper;
using AutoMapper.QueryableExtensions;
using Magnus.API.Data;
using Magnus.API.DTOs.Common;
using Magnus.API.DTOs.Departments;
using Magnus.API.Helpers;
using Magnus.API.Middleware;
using Magnus.API.Models;
using Magnus.API.Services.Interfaces;
using Magnus.API.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Magnus.API.Services.Implementations;

public class DepartmentService : IDepartmentService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;

    public DepartmentService(ApplicationDbContext context, IMapper mapper, IAuditService auditService)
    {
        _context = context;
        _mapper = mapper;
        _auditService = auditService;
    }

    public async Task<PagedResult<DepartmentReadDto>> GetPagedAsync(PagedQueryDto query, CancellationToken cancellationToken = default)
    {
        var departments = _context.Departments.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.Trim().ToLower();
            departments = departments.Where(x => x.Name.ToLower().Contains(searchTerm) || (x.Description != null && x.Description.ToLower().Contains(searchTerm)));
        }

        departments = query.SortBy?.ToLowerInvariant() switch
        {
            "createdat" => query.Descending ? departments.OrderByDescending(x => x.CreatedAt) : departments.OrderBy(x => x.CreatedAt),
            _ => query.Descending ? departments.OrderByDescending(x => x.Name) : departments.OrderBy(x => x.Name)
        };

        return await departments.ProjectTo<DepartmentReadDto>(_mapper.ConfigurationProvider).ToPagedResultAsync(query, cancellationToken);
    }

    public async Task<DepartmentReadDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var department = await _context.Departments.AsNoTracking()
            .ProjectTo<DepartmentReadDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return department ?? throw new AppException($"Department with id {id} was not found.", HttpStatusCode.NotFound);
    }

    public async Task<DepartmentReadDto> CreateAsync(DepartmentCreateDto request, CancellationToken cancellationToken = default)
    {
        var exists = await _context.Departments.AnyAsync(x => x.Name.ToLower() == request.Name.Trim().ToLower(), cancellationToken);
        if (exists)
        {
            throw new AppException("A department with the same name already exists.", HttpStatusCode.Conflict);
        }

        var department = _mapper.Map<Department>(request);
        _context.Departments.Add(department);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("Create", nameof(Department), department.Id.ToString(), $"Created department '{department.Name}'.", cancellationToken: cancellationToken);

        return _mapper.Map<DepartmentReadDto>(department);
    }

    public async Task<DepartmentReadDto> UpdateAsync(int id, DepartmentUpdateDto request, CancellationToken cancellationToken = default)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppException($"Department with id {id} was not found.", HttpStatusCode.NotFound);

        var duplicate = await _context.Departments.AnyAsync(x => x.Id != id && x.Name.ToLower() == request.Name.Trim().ToLower(), cancellationToken);
        if (duplicate)
        {
            throw new AppException("A department with the same name already exists.", HttpStatusCode.Conflict);
        }

        _mapper.Map(request, department);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("Update", nameof(Department), department.Id.ToString(), $"Updated department '{department.Name}'.", cancellationToken: cancellationToken);

        return _mapper.Map<DepartmentReadDto>(department);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppException($"Department with id {id} was not found.", HttpStatusCode.NotFound);

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("Delete", nameof(Department), department.Id.ToString(), $"Soft deleted department '{department.Name}'.", cancellationToken: cancellationToken);
    }
}
