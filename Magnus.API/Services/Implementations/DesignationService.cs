using AutoMapper;
using AutoMapper.QueryableExtensions;
using Magnus.API.Data;
using Magnus.API.DTOs.Common;
using Magnus.API.DTOs.Designations;
using Magnus.API.Helpers;
using Magnus.API.Middleware;
using Magnus.API.Models;
using Magnus.API.Services.Interfaces;
using Magnus.API.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Magnus.API.Services.Implementations;

public class DesignationService : IDesignationService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;

    public DesignationService(ApplicationDbContext context, IMapper mapper, IAuditService auditService)
    {
        _context = context;
        _mapper = mapper;
        _auditService = auditService;
    }

    public async Task<PagedResult<DesignationReadDto>> GetPagedAsync(PagedQueryDto query, CancellationToken cancellationToken = default)
    {
        var designations = _context.Designations.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.Trim().ToLower();
            designations = designations.Where(x => x.Title.ToLower().Contains(searchTerm) || (x.Description != null && x.Description.ToLower().Contains(searchTerm)));
        }

        designations = query.SortBy?.ToLowerInvariant() switch
        {
            "createdat" => query.Descending ? designations.OrderByDescending(x => x.CreatedAt) : designations.OrderBy(x => x.CreatedAt),
            _ => query.Descending ? designations.OrderByDescending(x => x.Title) : designations.OrderBy(x => x.Title)
        };

        return await designations.ProjectTo<DesignationReadDto>(_mapper.ConfigurationProvider).ToPagedResultAsync(query, cancellationToken);
    }

    public async Task<DesignationReadDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var designation = await _context.Designations.AsNoTracking()
            .ProjectTo<DesignationReadDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return designation ?? throw new AppException($"Designation with id {id} was not found.", HttpStatusCode.NotFound);
    }

    public async Task<DesignationReadDto> CreateAsync(DesignationCreateDto request, CancellationToken cancellationToken = default)
    {
        var exists = await _context.Designations.AnyAsync(x => x.Title.ToLower() == request.Title.Trim().ToLower(), cancellationToken);
        if (exists)
        {
            throw new AppException("A designation with the same title already exists.", HttpStatusCode.Conflict);
        }

        var designation = _mapper.Map<Designation>(request);
        _context.Designations.Add(designation);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("Create", nameof(Designation), designation.Id.ToString(), $"Created designation '{designation.Title}'.", cancellationToken: cancellationToken);

        return _mapper.Map<DesignationReadDto>(designation);
    }

    public async Task<DesignationReadDto> UpdateAsync(int id, DesignationUpdateDto request, CancellationToken cancellationToken = default)
    {
        var designation = await _context.Designations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppException($"Designation with id {id} was not found.", HttpStatusCode.NotFound);

        var duplicate = await _context.Designations.AnyAsync(x => x.Id != id && x.Title.ToLower() == request.Title.Trim().ToLower(), cancellationToken);
        if (duplicate)
        {
            throw new AppException("A designation with the same title already exists.", HttpStatusCode.Conflict);
        }

        _mapper.Map(request, designation);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("Update", nameof(Designation), designation.Id.ToString(), $"Updated designation '{designation.Title}'.", cancellationToken: cancellationToken);

        return _mapper.Map<DesignationReadDto>(designation);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var designation = await _context.Designations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new AppException($"Designation with id {id} was not found.", HttpStatusCode.NotFound);

        _context.Designations.Remove(designation);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("Delete", nameof(Designation), designation.Id.ToString(), $"Soft deleted designation '{designation.Title}'.", cancellationToken: cancellationToken);
    }
}
