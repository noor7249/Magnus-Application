using System.ComponentModel.DataAnnotations;

namespace Magnus.API.DTOs;

public class EmployeeUpdateDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [Required]
    public DateTime DateOfJoining { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Salary { get; set; }

    [Range(1, int.MaxValue)]
    public int DepartmentId { get; set; }

    [Range(1, int.MaxValue)]
    public int DesignationId { get; set; }

    public bool IsActive { get; set; }
}
