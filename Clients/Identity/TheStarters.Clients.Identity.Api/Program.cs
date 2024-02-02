using Serilog;
using TheStarters.Client.Common;
using TheStarters.Client.Common.Abstractions.DI;
using TheStarters.Client.Common.Middlewares;
using TheStarters.Client.Common.OpenApi;
using TheStarters.Clients.Identity.Api;
using TheStarters.Clients.Identity.Api.Context;
using TheStarters.Clients.Identity.Api.Options;
using TheStarters.Clients.Identity.Api.Services.Identity;
using Throw;

StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");
try
{
	var builder = WebApplication.CreateBuilder(args);
	builder.Host.UseSerilog((_, config) =>
	{
		config.WriteTo.Console()
			.ReadFrom.Configuration(builder.Configuration);
	});
	builder.Services.AddOpenApiDocumentation(builder.Configuration);
	builder.Services.AddHttpClient();
	builder.Services.AddServices();
	builder.Services.AddCurrentUser();
	builder.Services.AddPersistance(builder.Configuration);
	var securitySettings = builder.Configuration.GetSection(nameof(SecuritySettings)).Get<SecuritySettings>().ThrowIfNull();
	builder.Services.AddSingleton(securitySettings.Value);
	builder.Services.AddAuth(securitySettings.Value.JwtSettings.Key);
	builder.Services.AddAuthIdentity();
	builder.Services.AddRequestLogging(builder.Configuration)
		.AddExceptionMiddleware()
		.AddCors(opt => opt.AddPolicy("CorsPolicy", policy => policy.AllowAnyMethod()
			.SetIsOriginAllowed(_ => true)
			.AllowAnyHeader()
			.AllowCredentials()));
	builder.Services.AddControllers();
	var app = builder.Build();
	await app.InitDatabaseAsync<AppDbContext>();

	if (app.Environment.IsDevelopment())
	{
		app.UseOpenApiDocumentation(app.Configuration);
	}

	app.UseExceptionMiddleware();
	app.UseRouting();
	app.UseCors("CorsPolicy");
	app.UseAuthentication();
	app.UseAuthorization();
	app.UseCurrentUser();
	app.UseRequestLogging(app.Configuration);
	app.MapControllers();
	app.Run();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("StopTheHostException", StringComparison.Ordinal))
{
	StaticLogger.EnsureInitialized();
	Log.Fatal(ex, "Unhandled exception");
}
finally
{
	StaticLogger.EnsureInitialized();
	Log.Information("Server Shutting down...");
	Log.CloseAndFlush();
}

