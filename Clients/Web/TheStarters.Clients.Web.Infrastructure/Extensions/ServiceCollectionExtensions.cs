using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheStarters.Clients.Web.Application.Abstractions.DI;
using TheStarters.Clients.Web.Application.ClientSubscriptions;
using TheStarters.Clients.Web.Infrastructure.Context;
using TheStarters.Clients.Web.Infrastructure.Identity;
using TheStarters.Clients.Web.Infrastructure.Middlewares;
using TheStarters.Clients.Web.Infrastructure.Notifications;
using TheStarters.Clients.Web.Infrastructure.OpenApi;

namespace TheStarters.Clients.Web.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
	{
		return services
			.AddClientSubscriptions()
			.AddNotifications()
			.AddAuth(config)
			.AddAuthIdentity()
			.AddCurrentUser()
			.AddPersistance(config)
			.AddRequestLogging(config)
			.AddExceptionMiddleware()
			.AddOpenApiDocumentation(config)
			.AddServices()
			.AddCors(opt => opt.AddPolicy("CorsPolicy", policy => policy.AllowAnyMethod()
				.SetIsOriginAllowed(_ => true)
				.AllowAnyHeader()
				.AllowCredentials()));
		
	}
	public static async Task UseInfrastructureAsync(this IApplicationBuilder app, IConfiguration config)
	{
		await app.InitDatabaseAsync<AppDbContext>();
		app.UseExceptionMiddleware();
		app.UseRouting();
		app.UseCors("CorsPolicy");
		app.UseAuthentication();
		app.UseAuthorization();
		app.UseCurrentUser();
		app.UseRequestLogging(config);
		app.UseOpenApiDocumentation(config);
	}
	
	public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
	{
		builder.MapControllers();
		builder.MapNotifications();
		builder.MapFallbackToFile("index.html");
		return builder;
	}
}