using System.Net;
using Orleans.Configuration;
using StackExchange.Redis;
using TheStarters.Server.Grains.Consts;

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
		.UseOrleans((_, silo) =>
		{
			silo
				//.UseLocalhostClustering()
				.UseZooKeeperClustering(options => { options.ConnectionString = "127.0.0.1:30000"; })
				.Configure<ClusterOptions>(options =>
				{
					options.ClusterId = "dev";
					options.ServiceId = "OrleansTestServer";
				})
				.AddRedisGrainStorage(StorageConsts.PersistenceStorage, options =>
				{
					options.ConfigurationOptions = new()
					{
						EndPoints = new EndPointCollection()
						{
							{ IPAddress.Parse("192.168.31.103"), 6388 }
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