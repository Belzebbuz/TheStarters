using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Orleans.Core;
using Orleans.Runtime;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Abstractions.Monopoly;
using TheStarters.Server.Grains.Consts;

namespace TheStarters.Server.Grains;

public class GameFactoryGrain(
	[PersistentState(stateName: "gamesIds", storageName: StorageConsts.PersistenceStorage)]
	IStorage<ConcurrentDictionary<GameType, long>> gameIds,
	IGrainFactory client,
	ILogger<GameFactoryGrain> logger)
	: ObservableGrain<IGameFactoryObserver>(logger), IGameFactoryGrain
{
	public async ValueTask<long> CreateGameAsync(GameType gameType, Guid userId)
	{
		gameIds.State.TryAdd(gameType, 1);
		gameIds.State[gameType] = ++gameIds.State[gameType];
		var gameId = gameIds.State[gameType];
		var game = default(BaseGame);
		switch (gameType)
		{
			case GameType.TicTacToe:
				game = await client.GetGrain<ITicTacToeGrain>(gameId).InitStateAsync(userId);
				break;
			case GameType.Monopoly:
				game = await client.GetGrain<IMonopolyGrain>(gameId).InitStateAsync(userId);
				break;
			default:
				throw new NotImplementedException("Такой тип игры не поддерживается");
		}
		
		await client.GetGrain<IPlayerGrain>(userId).JoinGameAsync(game.GameType, gameId);
		await client.GetGrain<IGamesListGrain>(Guid.Empty).AddOrUpdateGameAsync(game);
		await gameIds.WriteStateAsync();
		await NotifyAsync(gameType, game.Id);
		return game.Id;
	}
	private ValueTask NotifyAsync(GameType gameType, long id)
	{
		ObserverManager.Notify(obs => obs.GameCreated(gameType, id));
		return ValueTask.CompletedTask;
	}
}