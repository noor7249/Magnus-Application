using AutoMapper;
using Magnus.API.DTOs;
using Magnus.API.DTOs.AuditLogs;
using Magnus.API.DTOs.Departments;
using Magnus.API.DTOs.Designations;
using Magnus.API.Models;

namespace Magnus.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Department, DepartmentReadDto>();
        CreateMap<DepartmentCreateDto, Department>();
        CreateMap<DepartmentUpdateDto, Department>();

        CreateMap<Designation, DesignationReadDto>();
        CreateMap<DesignationCreateDto, Designation>();
        CreateMap<DesignationUpdateDto, Designation>();

        CreateMap<Employee, EmployeeReadDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty))
            .ForMember(dest => dest.DesignationTitle, opt => opt.MapFrom(src => src.Designation != null ? src.Designation.Title : string.Empty));
        CreateMap<EmployeeCreateDto, Employee>();
        CreateMap<EmployeeUpdateDto, Employee>();

        CreateMap<AuditLog, AuditLogReadDto>();
    }
}
