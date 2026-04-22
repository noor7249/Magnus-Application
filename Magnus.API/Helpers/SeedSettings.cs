using System.ComponentModel.DataAnnotations;

namespace Magnus.API.Helpers;

public class SeedSettings
{
    public const string SectionName = "SeedSettings";

    [Required]
    public string FullName { get; set; } = "System Administrator";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "admin@magnus.com";

    [Required]
    public string Username { get; set; } = "admin";

    [Required]
    [MinLength(10, ErrorMessage = "SeedSettings:AdminPassword must be at least 10 characters to satisfy the Identity password policy.")]
    public string AdminPassword { get; set; } = string.Empty;

    public bool ResetAdminPasswordOnSeed { get; set; }
}
