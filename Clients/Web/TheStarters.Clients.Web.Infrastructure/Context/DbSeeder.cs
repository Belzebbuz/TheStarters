using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheStarters.Clients.Web.Application.Settings.Auth;
using TheStarters.Clients.Web.Infrastructure.Constants;
using TheStarters.Clients.Web.Infrastructure.Identity.Models;

namespace TheStarters.Clients.Web.Infrastructure.Context;
public class DbSeeder
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<DbSeeder> _logger;
    private readonly SecuritySettings _securitySettings;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DbSeeder(UserManager<AppUser> userManager,
        SecuritySettings securitySettings,
        ILogger<DbSeeder> logger, RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
        _securitySettings = securitySettings;
    }

    public async Task SeedDataAsync()
    {
        foreach (var roleName in Roles.DefaultRoles)
        {
            if (await _roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName)
                is { } role) continue;
            
            role = new IdentityRole()
            {
                Name = roleName,
            };
            await _roleManager.CreateAsync(role);
            _logger.LogInformation("Seeding {role} Role", roleName);
        }
        if (await _userManager.Users.SingleOrDefaultAsync(x => x.Email == _securitySettings.RootUserEmail) == null)
        {
            var adminUser = new AppUser()
            {
                UserName = _securitySettings.RootUserEmail,
                Email = _securitySettings.RootUserEmail,
                EmailConfirmed = true
            };
            await _userManager.CreateAsync(adminUser, _securitySettings.DefaultPassword);
            
            _logger.LogInformation($"Root user {adminUser.Email} created");
        }
        var admin = await _userManager.FindByEmailAsync( _securitySettings.RootUserEmail );
        if (admin == null)
            return;
        var roles = await _roleManager.Roles.ToListAsync();
        foreach (var role in roles)
        {
            if (await _userManager.IsInRoleAsync(admin, role.Name)) continue;
            await _userManager.AddToRoleAsync(admin, role.Name);
            _logger.LogInformation($"Root user  {admin.Email} added to role {role.Name}");
        }
        if(!admin.IsActive)
        {
            admin.IsActive = true;
            await _userManager.UpdateAsync(admin);
        }
    }
}
