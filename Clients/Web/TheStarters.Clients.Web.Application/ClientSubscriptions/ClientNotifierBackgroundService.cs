using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Clients.Web.Application.ClientSubscriptions;

public class ClientNotifierBackgroundService(
	SubRequestChannel requestChannel, 
	IServiceProvider provider, 
	ILogger<ClientNotifierBackgroundService> logger, 
	IGrainFactory client)
	: BackgroundService
{
	private readonly ConcurrentDictionary<GameSubscribe, IGrainObserver> _activeSubscribes = new();
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await LoadStartedGamesAsync();
		await foreach (var request in requestChannel.Requests.Reader.ReadAllAsync(stoppingToken))
		{
			var _ = request switch
			{
				GameSubscribe subscribe => SubscribeGameAsync(subscribe),
				GameUnsubscribe unsubscribe => UnsubscribeGameAsync(unsubscribe),
				_ => Task.CompletedTask,
			};
		}
	}

	private async Task LoadStartedGamesAsync()
	{
		var lastUpdatedGames = await client.GetGrain<IGamesGrain>(Guid.Empty).GetLastUpdatedAsync();
		foreach (var subscribe in lastUpdatedGames.Value.Select(game => new GameSubscribe(game.GameType, game.Id)))
		{
			await SubscribeGameAsync(subscribe);
		}
	}

	private async Task SubscribeGameAsync(GameSubscribe request)
	{
		if (request.GameType == GameType.TicTacToe)
		{
			var observer = provider.GetRequiredService<TicTacToeObserver>();
			observer.GameId = request.GameId;
			var reference = client.CreateObjectReference<ITicTacToeObserver>(observer);
			await client.GetGrain<ITicTacToeGrain>(request.GameId).SubscribeAsync(reference);
			_activeSubscribes.TryAdd(request, reference);
			logger.LogInformation($"Подписка для игры №{request.GameId} создана.");
		}
	}
	
	private async Task UnsubscribeGameAsync(GameUnsubscribe request)
	{
			var key = new GameSubscribe(request.GameType, request.GameId);
			if(!_activeSubscribes.TryRemove(key, out var reference))
			{
				logger.LogWarning($"Подписка для игры №{request.GameId} не найдена.");
				return;
			}
			
			if (request.GameType == GameType.TicTacToe && reference is ITicTacToeObserver obj)
				await client.GetGrain<ITicTacToeGrain>(request.GameId).UnsubscribeAsync(obj);

	}
}