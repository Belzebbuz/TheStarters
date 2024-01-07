using System.Collections.Concurrent;
using Orleans.Runtime;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Grains.Consts;

namespace TheStarters.Server.Grains;

public class GameFactoryGrain : IGameFactoryGrain
{
	private readonly IPersistentState<ConcurrentDictionary<GameType, long>> _gameIds;
	private readonly IGrainFactory _client;

	public GameFactoryGrain(
		[PersistentState(stateName:"gamesIds", storageName: StorageConsts.PersistenceStorage)] 
		IPersistentState<ConcurrentDictionary<GameType, long>> gameIds,
		IGrainFactory client)
	{
		_gameIds = gameIds;
		_client = client;
	}
	public async ValueTask<long> CreateGameAsync(GameType gameType, Guid userId)
	{
		_gameIds.State.TryAdd(gameType, 1);
		_gameIds.State[gameType] = ++_gameIds.State[gameType];
		var game = new TicTacToeGame()
		{
			Id = _gameIds.State[gameType],
			Player1 = userId,
			CreatedOn = DateTime.UtcNow,
			GameType = gameType
		};
		await _client.GetGrain<ITicTacToeGrain>(game.Id).UpdateAsync(game);
		await _client.GetGrain<IPlayerGrain>(userId).AddOrUpdateGameAsync(game);
		await _client.GetGrain<IGamesGrain>(Guid.Empty).AddOrUpdateGameAsync(game);
		await _gameIds.WriteStateAsync();
		return game.Id;
	}
}