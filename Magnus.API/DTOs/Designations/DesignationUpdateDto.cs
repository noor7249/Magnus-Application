using System.ComponentModel.DataAnnotations;

namespace Magnus.API.DTOs.Designations;

public class DesignationUpdateDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}
