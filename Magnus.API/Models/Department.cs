using Magnus.API.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Magnus.API.Models;

public class Department : AuditableEntity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
