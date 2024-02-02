using System.Net;
using Orleans.Configuration;
using Serilog;
using StackExchange.Redis;
using TheStarters.Server.Grains.Consts;
using TheStarters.Server.Silo;
using TheStarters.Server.Silo.Settings;
using Throw;
EnsureInitialized();
try
{
	var builder = WebApplication.CreateBuilder();
	builder.Host.UseSerilog((_, config) =>
	{
		config.WriteTo.Console()
			.ReadFrom.Configuration(builder.Configuration);
	});
	builder.Host.AddOrleans();
	var app = builder.Build();
	app.Run();
	Log.Information("\n Press Enter to terminate...");
}
catch (Exception e)
{
	EnsureInitialized();
	Log.Fatal(e, "Unhandled exception");
}
finally
{
	EnsureInitialized();
	Log.Information("Server Shutting down...");
	Log.CloseAndFlush();
}

static void EnsureInitialized()
{
	if (Log.Logger is not Serilog.Core.Logger)
		Log.Logger = new LoggerConfiguration()
			.Enrich.FromLogContext()
			.WriteTo.Console()
			.CreateLogger();
}