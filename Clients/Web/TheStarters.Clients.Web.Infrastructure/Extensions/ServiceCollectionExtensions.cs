using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheStarters.Client.Common;
using TheStarters.Client.Common.Abstractions.DI;
using TheStarters.Client.Common.Middlewares;
using TheStarters.Client.Common.OpenApi;
using TheStarters.Clients.Web.Application.ClientSubscriptions;
using TheStarters.Clients.Web.Infrastructure.Context;
using TheStarters.Clients.Web.Infrastructure.Notifications;
using Throw;

namespace TheStarters.Clients.Web.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
	{
		return services
			.AddHttpClient()
			.AddClientSubscriptions()
			.AddNotifications()
			.AddAuth(config["jwt-key"].ThrowIfNull())
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