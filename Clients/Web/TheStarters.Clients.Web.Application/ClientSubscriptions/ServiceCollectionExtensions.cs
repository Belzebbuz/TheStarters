using Microsoft.Extensions.DependencyInjection;
using TheStarters.Server.Abstractions;

namespace TheStarters.Clients.Web.Application.ClientSubscriptions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddClientSubscriptions(this IServiceCollection services)
	{
		services.AddHostedService<ClientNotifierBackgroundService>();
		services.AddSingleton<SubRequestChannel>();
		services.AddTransient<TicTacToeObserver>();
		return services;
	}
}