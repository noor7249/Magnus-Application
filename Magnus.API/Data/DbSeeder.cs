using Magnus.API.Helpers;
using Magnus.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Magnus.API.Data;

public static class DbSeeder
{
    public static async Task SeedIdentityAsync(IServiceProvider serviceProvider, ILogger? logger = null)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var seedSettings = serviceProvider.GetRequiredService<IOptions<SeedSettings>>().Value;

        if (string.IsNullOrWhiteSpace(seedSettings.AdminPassword) || seedSettings.AdminPassword.Contains("__SET_IN_USER_SECRETS_OR_ENV__", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("SeedSettings:AdminPassword must be provided through user secrets or environment variables.");
        }

        foreach (var role in new[] { RoleConstants.Admin, RoleConstants.Manager, RoleConstants.Employee })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join("; ", roleResult.Errors.Select(x => x.Description));
                    throw new InvalidOperationException($"Role seeding failed for '{role}': {errors}");
                }

                logger?.LogInformation("Seeded role {RoleName}.", role);
            }
        }

        var adminUser = await userManager.FindByEmailAsync(seedSettings.Email);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                FullName = seedSettings.FullName,
                Email = seedSettings.Email,
                UserName = seedSettings.Username,
                EmailConfirmed = true,
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(adminUser, seedSettings.AdminPassword);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(x => x.Description));
                throw new InvalidOperationException($"Admin user seeding failed: {errors}");
            }

            logger?.LogInformation("Seeded admin user {AdminEmail} with username {AdminUsername}.", seedSettings.Email, seedSettings.Username);
        }

        if (!await userManager.IsInRoleAsync(adminUser, RoleConstants.Admin))
        {
            var addToRoleResult = await userManager.AddToRoleAsync(adminUser, RoleConstants.Admin);
            if (!addToRoleResult.Succeeded)
            {
                var errors = string.Join("; ", addToRoleResult.Errors.Select(x => x.Description));
                throw new InvalidOperationException($"Adding admin user to role failed: {errors}");
            }

            logger?.LogInformation("Assigned role {RoleName} to admin user {AdminEmail}.", RoleConstants.Admin, seedSettings.Email);
        }
    }
}
