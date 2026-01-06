using Microsoft.AspNetCore.Identity;

namespace HQS.Infrastructure.Data.Seed;

public static class RoleSeeder
{
    private static readonly string[] Roles =
    {
        "MasterAdmin",
        "HospitalRep"
    };

    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
