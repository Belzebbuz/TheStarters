
using Orleans.Configuration;
using Serilog;
using TheStarters.Clients.Web.Api;
using TheStarters.Clients.Web.Application.Settings.Orleans;
using TheStarters.Clients.Web.Infrastructure.Extensions;
using TheStarters.Clients.Web.Infrastructure.JsonConverters;
using Throw;

StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");

try
{
	var builder = WebApplication.CreateBuilder();
	builder.Host.AddConfigurations();
	builder.Host.UseOrleansClient(client =>
	{
		var orleansSettings = builder.Configuration.GetSection(nameof(OrleansSettings)).Get<OrleansSettings>();
		orleansSettings.ThrowIfNull();
		client
			//.UseLocalhostClustering()
			.UseZooKeeperClustering(options =>
			{
				options.ConnectionString = orleansSettings.ConnectionString;
			})
			.Configure<ClusterOptions>(options =>
			{
				options.ClusterId = orleansSettings.ClusterId;
				options.ServiceId = orleansSettings.ServiceId;
			});
	});
	builder.Host.UseSerilog((_, config) =>
	{
		config.WriteTo.Console()
			.ReadFrom.Configuration(builder.Configuration);
	});

	builder.Services.AddControllers().AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.Converters.Add(new NullableGuidArrayConverter());
	});
	builder.Services.AddInfrastructure(builder.Configuration);
	var app = builder.Build();
	await app.UseInfrastructureAsync(app.Configuration);
	app.UseStaticFiles();
	app.MapEndpoints();
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
