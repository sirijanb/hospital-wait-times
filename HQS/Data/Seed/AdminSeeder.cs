using HQS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace HQS.Infrastructure.Data.Seed;

public static class AdminSeeder
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        const string adminEmail = "masteradmin@hqs.com";
        const string adminPassword = "Admin@12345"; // change after first login

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser != null)
            return;
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            IsApproved = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (!result.Succeeded)
        {
            throw new Exception("Failed to create MasterAdmin user");
        }
        else
        {
            Console.WriteLine("*** Master admin created ***");
        }
        await userManager.AddToRoleAsync(adminUser, "MasterAdmin");
    }
}
