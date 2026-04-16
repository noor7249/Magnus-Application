using AutoMapper;
using AutoMapper.QueryableExtensions;
using Magnus.API.Data;
using Magnus.API.DTOs;
using Magnus.API.DTOs.Common;
using Magnus.API.Helpers;
using Magnus.API.Middleware;
using Magnus.API.Models;
using Magnus.API.Services.Interfaces;
using Magnus.API.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Magnus.API.Services.Implementations;

public class EmployeeService : IEmployeeService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;

    public EmployeeService(ApplicationDbContext context, IMapper mapper, IAuditService auditService)
    {
        _context = context;
        _mapper = mapper;
        _auditService = auditService;
    }

    public async Task<PagedResult<EmployeeReadDto>> GetPagedAsync(PagedQueryDto query, CancellationToken cancellationToken = default)
    {
        var employees = _context.Employees
            .AsNoTracking()
            .Include(x => x.Department)
            .Include(x => x.Designation)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.Trim().ToLower();
            employees = employees.Where(x => x.FirstName.ToLower().Contains(searchTerm) || x.LastName.ToLower().Contains(searchTerm) || x.Email.ToLower().Contains(searchTerm));
        }

        employees = query.SortBy?.ToLowerInvariant() switch
        {
            "email" => query.Descending ? employees.OrderByDescending(x => x.Email) : employees.OrderBy(x => x.Email),
            "dateofjoining" => query.Descending ? employees.OrderByDescending(x => x.DateOfJoining) : employees.OrderBy(x => x.DateOfJoining),
            _ => query.Descending ? employees.OrderByDescending(x => x.CreatedAt) : employees.OrderBy(x => x.CreatedAt)
        };

        return await employees.ProjectTo<EmployeeReadDto>(_mapper.ConfigurationProvider).ToPagedResultAsync(query, cancellationToken);
    }

    public async Task<EmployeeReadDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _context.Employees.AsNoTracking()
            .Include(x => x.Department)
            .Include(x => x.Designation)
            .ProjectTo<EmployeeReadDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return employee ?? throw new AppException($"Employee with id {id} was not found.", HttpStatusCode.NotFound);
    }

    public async Task<EmployeeReadDto> CreateAsync(EmployeeCreateDto request, CancellationToken cancellationToken = default)
    {
        await EnsureReferencesExistAsync(request.DepartmentId, request.DesignationId, cancellationToken);

        var exists = await _context.Employees.AnyAsync(x => x.Email.ToLower() == request.Email.Trim().ToLower(), cancellationToken);
        if (exists)
        {
            throw new AppException("An employee with the same email already exists.", HttpStatusCode.Conflict);
        }

        var employee = _mapper.Map<Employee>(request);
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("Create", nameof(Employee), employee.Id.ToString(), $"Created employee '{employee.Email}'.", cancellationToken: cancellationToken);

        return await GetByIdAsync(employee.Id, cancellationToken);
    }

    public async Task<EmployeeReadDto> UpdateAsync(int id, EmployeeUpdateDto request, CancellationToken cancellationToken = default)
    {
        await EnsureReferencesExistAsync(request.DepartmentId, request.DesignationId, cancellationToken);

        var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppException($"Employee with id {id} was not found.", HttpStatusCode.NotFound);

        var duplicate = await _context.Employees.AnyAsync(x => x.Id != id && x.Email.ToLower() == request.Email.Trim().ToLower(), cancellationToken);
        if (duplicate)
        {
            throw new AppException("An employee with the same email already exists.", HttpStatusCode.Conflict);
        }

        _mapper.Map(request, employee);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("Update", nameof(Employee), employee.Id.ToString(), $"Updated employee '{employee.Email}'.", cancellationToken: cancellationToken);

        return await GetByIdAsync(employee.Id, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppException($"Employee with id {id} was not found.", HttpStatusCode.NotFound);

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("Delete", nameof(Employee), employee.Id.ToString(), $"Soft deleted employee '{employee.Email}'.", cancellationToken: cancellationToken);
    }

    private async Task EnsureReferencesExistAsync(int departmentId, int designationId, CancellationToken cancellationToken)
    {
        if (!await _context.Departments.AnyAsync(x => x.Id == departmentId, cancellationToken))
        {
            throw new AppException($"Department with id {departmentId} was not found.", HttpStatusCode.BadRequest);
        }

        if (!await _context.Designations.AnyAsync(x => x.Id == designationId, cancellationToken))
        {
            throw new AppException($"Designation with id {designationId} was not found.", HttpStatusCode.BadRequest);
        }
    }
}
