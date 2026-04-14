using AutoMapper;
using Magnus.API.DTOs;
using Magnus.API.Models;

namespace Magnus.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Employee, EmployeeReadDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty))
            .ForMember(dest => dest.DesignationTitle, opt => opt.MapFrom(src => src.Designation != null ? src.Designation.Title : string.Empty));

        CreateMap<EmployeeCreateDto, Employee>();
        CreateMap<EmployeeUpdateDto, Employee>();
    }
}
