using System.Net;
using Orleans.Configuration;
using StackExchange.Redis;
using TheStarters.Server.Grains.Consts;
using TheStarters.Server.Silo;
using TheStarters.Server.Silo.Settings;
using Throw;

try
{
	using var host = await StartSiloAsync();
	Console.WriteLine("\n Press Enter to terminate...");
	Console.ReadLine();
	await host.StopAsync();
	return 0;
}
catch (Exception e)
{
	Console.WriteLine(e);
	return 1;
}


static async Task<IHost> StartSiloAsync()
{
	var builder = WebApplication.CreateBuilder();
	builder.Host
		.UseOrleans((hostBuilder, silo) =>
		{
			var siloSettings = hostBuilder.Configuration.GetSection(nameof(SiloSettings)).Get<SiloSettings>();
			siloSettings.ThrowIfNull("Не установлены настройки Silo");
			silo
				//.UseLocalhostClustering()
				.UseZooKeeperClustering(options => { options.ConnectionString = siloSettings.ClusterSettings.ConnectionString; })
				.Configure<ClusterOptions>(options =>
				{
					options.ClusterId = siloSettings.ClusterSettings.ClusterId;
					options.ServiceId = siloSettings.ClusterSettings.ServiceId;
				})
				.AddRedisGrainStorage(StorageConsts.PersistenceStorage, options =>
				{
					options.ConfigurationOptions = new()
					{
						EndPoints = new EndPointCollection()
						{
							{ IPAddress.Parse(siloSettings.RedisPersistence.IPAddress), siloSettings.RedisPersistence.Port }
						}
					};
				})
				.ConfigureLogging(logging => logging.AddConsole())
				.UseDashboard(options =>
				{
					options.Username = "username";
					options.Password = "password";
					options.Host = "*";
					options.Port = 9000;
					options.HostSelf = true;
					options.CounterUpdateIntervalMs = 1000;
				});
		});

	var host = builder.Build();
	await host.StartAsync();
	return host;
}