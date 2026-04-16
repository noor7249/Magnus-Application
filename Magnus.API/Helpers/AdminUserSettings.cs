namespace Magnus.API.Helpers;

public class AdminUserSettings
{
    public const string SectionName = "AdminUser";

    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
