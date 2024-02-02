using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TheStarters.Clients.Web.Application.ClientSubscriptions.Observers;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Abstractions.Monopoly;

namespace TheStarters.Clients.Web.Application.ClientSubscriptions;

public class ClientNotifierBackgroundService(
	SubRequestChannel requestChannel, 
	IServiceProvider provider, 
	ILogger<ClientNotifierBackgroundService> logger, 
	IGrainFactory client)
	: BackgroundService
{
	private readonly ConcurrentDictionary<GameSubscribe, IGrainObserver> _activeSubscribes = new();
	private readonly ConcurrentBag<IGrainObserver> _observers = new();
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await LoadStartedGamesAsync();
		await SubscribeGameListAsync();
		await SubscribeGameFactoryAsync();
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

	private async Task SubscribeGameFactoryAsync()
	{
		var observer = provider.GetRequiredService<GameFactoryObserver>();
		var reference = client.CreateObjectReference<IGameFactoryObserver>(observer);
		await client.GetGrain<IGameFactoryGrain>(Guid.Empty).SubscribeAsync(reference);
		logger.LogInformation("Создана подписка на создание игры");
		_observers.Add(reference);
	}

	private async Task SubscribeGameListAsync()
	{
		var observer = provider.GetRequiredService<GameListObserver>();
		var reference = client.CreateObjectReference<IGamesListObserver>(observer);
		await client.GetGrain<IGamesListGrain>(Guid.Empty).SubscribeAsync(reference);
		logger.LogInformation("Создана подписка на обновление списка игр");
		_observers.Add(reference);
	}

	private async Task LoadStartedGamesAsync()
	{
		var lastUpdatedGames = await client.GetGrain<IGamesListGrain>(Guid.Empty).GetLastUpdatedAsync();
		foreach (var subscribe in lastUpdatedGames.Value.Select(game => new GameSubscribe(game.GameType, game.Id)))
		{
			await SubscribeGameAsync(subscribe);
		}
	}

	private async Task SubscribeGameAsync(GameSubscribe request)
	{
		switch (request.GameType)
		{
			case GameType.TicTacToe:
			{
				var observer = provider.GetRequiredService<TicTacToeObserver>();
				observer.GameId = request.GameId;
				var reference = client.CreateObjectReference<ITicTacToeObserver>(observer);
				await client.GetGrain<ITicTacToeGrain>(request.GameId).SubscribeAsync(reference);
				_activeSubscribes.TryAdd(request, reference);
				logger.LogInformation($"Подписка для игры №{request.GameId} создана.");
				break;
			}
			case GameType.Monopoly:
			{
				var observer = provider.GetRequiredService<MonopolyObserver>();
				observer.GameId = request.GameId;
				var reference = client.CreateObjectReference<IMonopolyObserver>(observer);
				await client.GetGrain<IMonopolyGrain>(request.GameId).SubscribeAsync(reference);
				_activeSubscribes.TryAdd(request, reference);
				logger.LogInformation($"Подписка для игры №{request.GameId} создана.");
				break;
			}
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

			switch (request.GameType)
			{
				case GameType.TicTacToe when reference is ITicTacToeObserver ticObj:
					await client.GetGrain<ITicTacToeGrain>(request.GameId).UnsubscribeAsync(ticObj);
					break;
				case GameType.Monopoly when reference is IMonopolyObserver monoObj:
					await client.GetGrain<IMonopolyGrain>(request.GameId).UnsubscribeAsync(monoObj);
					break;
			}
	}
}