using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Magnus.API.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = "e6f5b9b0-8f3d-4cb8-a0f1-3f7dd7f4d101",
                ConcurrencyStamp = "739a1516-fc41-433c-991d-940619ee8c58",
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {
                Id = "e6f5b9b0-8f3d-4cb8-a0f1-3f7dd7f4d102",
                ConcurrencyStamp = "a3d4d688-6255-48a5-8aa8-a0db8c8c22ea",
                Name = "Manager",
                NormalizedName = "MANAGER"
            },
            new IdentityRole
            {
                Id = "e6f5b9b0-8f3d-4cb8-a0f1-3f7dd7f4d103",
                ConcurrencyStamp = "25aaa1f3-7130-49ab-91e2-025942772e9c",
                Name = "Employee",
                NormalizedName = "EMPLOYEE"
            });
    }
}
