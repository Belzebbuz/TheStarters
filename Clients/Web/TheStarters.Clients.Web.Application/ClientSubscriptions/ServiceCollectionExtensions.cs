using Microsoft.Extensions.DependencyInjection;
using TheStarters.Clients.Web.Application.ClientSubscriptions.Observers;

namespace TheStarters.Clients.Web.Application.ClientSubscriptions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddClientSubscriptions(this IServiceCollection services)
	{
		services.AddHostedService<ClientNotifierBackgroundService>();
		services.AddSingleton<SubRequestChannel>();
		services.AddTransient<TicTacToeObserver>();
		services.AddTransient<MonopolyObserver>();
		services.AddTransient<GameListObserver>();
		services.AddTransient<GameFactoryObserver>();
		return services;
	}
}