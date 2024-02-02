using Orleans.Configuration;
using StackExchange.Redis;
using TheStarters.Server.Grains.Consts;
using TheStarters.Server.Silo.Settings;
using Throw;

namespace TheStarters.Server.Silo;

public static class HostBuilderExtensions
{
	internal static IHostBuilder AddOrleans(this IHostBuilder builder)
	{
		return builder.UseOrleans((hostBuilder, silo) =>
		{
			var siloSettings = hostBuilder.Configuration.GetSection(nameof(SiloSettings)).Get<SiloSettings>();
			siloSettings.ThrowIfNull("Не установлены настройки Silo");
			silo
				//.UseLocalhostClustering()
				.UseRedisClustering(options => options.ConfigurationOptions = new()
				{
					EndPoints = new EndPointCollection()
					{
						new(siloSettings.ClusterSettings.ConnectionString)
					}
				})
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
							new(siloSettings.RedisPersistence.ConnectionString)
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
	}
}