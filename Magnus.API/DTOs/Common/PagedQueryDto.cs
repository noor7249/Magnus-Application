using System.ComponentModel.DataAnnotations;

namespace Magnus.API.DTOs.Common;

public class PagedQueryDto
{
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    [MaxLength(100)]
    public string? SearchTerm { get; set; }

    [MaxLength(50)]
    public string? SortBy { get; set; }

    public bool Descending { get; set; }
}
