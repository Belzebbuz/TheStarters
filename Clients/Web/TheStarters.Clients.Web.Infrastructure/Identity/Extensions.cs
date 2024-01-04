using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TheStarters.Clients.Web.Infrastructure.Context;
using TheStarters.Clients.Web.Infrastructure.Identity.Models;

namespace TheStarters.Clients.Web.Infrastructure.Identity;

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