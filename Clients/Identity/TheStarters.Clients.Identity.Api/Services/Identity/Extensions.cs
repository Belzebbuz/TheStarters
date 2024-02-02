using Microsoft.AspNetCore.Identity;
using TheStarters.Clients.Identity.Api.Context;
using TheStarters.Clients.Identity.Api.Services.Identity.Models;

namespace TheStarters.Clients.Identity.Api.Services.Identity;

public static class Extensions
{
	public static IServiceCollection AddAuthIdentity(this IServiceCollection services) =>
		services
			.AddIdentity<AppUser, IdentityRole>(options =>
			{
				options.Password.RequiredLength = 8;
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.User.RequireUniqueEmail = true;
			})
			.AddEntityFrameworkStores<AppDbContext>()
			.AddDefaultTokenProviders()
			.Services;
}