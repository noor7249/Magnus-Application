using System.ComponentModel.DataAnnotations;

namespace Magnus.API.DTOs.Departments;

public class DepartmentUpdateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}
