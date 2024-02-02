using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheStarters.Clients.Identity.Api.Constants;
using TheStarters.Clients.Identity.Api.Options;
using TheStarters.Clients.Identity.Api.Services.Identity.Models;
using Throw;

namespace TheStarters.Clients.Identity.Api.Context;
public class DbSeeder(
    UserManager<AppUser> userManager,
    SecuritySettings securitySettings,
    ILogger<DbSeeder> logger,
    RoleManager<IdentityRole> roleManager)
{
    public async Task SeedDataAsync()
    {
        foreach (var roleName in Roles.DefaultRoles)
        {
            var role = await roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName);
            if (role != null) continue;
            
            role = new IdentityRole()
            {
                Name = roleName,
            };
            await roleManager.CreateAsync(role);
            logger.LogInformation("Seeding {role} Role", roleName);
        }
        if (await userManager.Users.SingleOrDefaultAsync(x => x.Email == securitySettings.RootUserEmail) == null)
        {
            var adminUser = new AppUser()
            {
                UserName = securitySettings.RootUserEmail,
                Email = securitySettings.RootUserEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, securitySettings.DefaultPassword);
            
            logger.LogInformation($"Root user {adminUser.Email} created");
        }
        var admin = await userManager.FindByEmailAsync( securitySettings.RootUserEmail );
        if (admin == null)
            return;
        var roles = await roleManager.Roles.ToListAsync();
        foreach (var role in roles)
        {
            if (await userManager.IsInRoleAsync(admin, role.Name.ThrowIfNull(nameof(role.Name)))) continue;
            await userManager.AddToRoleAsync(admin, role.Name);
            logger.LogInformation($"Root user  {admin.Email} added to role {role.Name}");
        }
        if(!admin.IsActive)
        {
            admin.IsActive = true;
            await userManager.UpdateAsync(admin);
        }
    }
}
