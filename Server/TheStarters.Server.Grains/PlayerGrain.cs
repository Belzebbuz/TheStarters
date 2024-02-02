using Microsoft.Extensions.Logging;
using Orleans.Core;
using Orleans.Runtime;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Grains.Consts;
using Throw;

namespace TheStarters.Server.Grains;

public class PlayerGrain(
	[PersistentState(stateName: "profile", storageName: StorageConsts.PersistenceStorage)]
	IStorage<PlayerProfile> player,
	[PersistentState(stateName: "player-games", storageName: StorageConsts.PersistenceStorage)]
	IStorage<Dictionary<GameType, HashSet<long>>> games,
	ILogger<PlayerGrain> logger)
	: ObservableGrain<IPlayerObserver>(logger), IPlayerGrain
{
	private readonly ILogger<PlayerGrain> _logger = logger;

	public ValueTask<PlayerProfile> GetProfileAsync()
		=> ValueTask.FromResult(player.State);

	public async Task SetNameAsync(string name)
	{
		name.ThrowIfNull().IfEmpty();
		var id = this.GetPrimaryKey();
		player.State.Name = name;
		player.State.Id = id;
		await player.WriteStateAsync();
		ObserverManager.Notify(x => x.StateHasChanged());
	}

	public async Task JoinGameAsync(GameType gameType, long gameId)
	{
		var games1 = games.State.GetValueOrDefault(gameType);
		if (games1 is null)
		{
			games1 = new();
			games.State[gameType] = games1;
		}

		games1.Add(gameId);
		await games.WriteStateAsync();
		ObserverManager.Notify(x => x.StateHasChanged());
	}

	public async Task RemoveFromGameAsync(GameType gameType, long gameId)
	{
		var games1 = games.State.GetValueOrDefault(gameType);
		games1.ThrowIfNull($"Пользователь не был добавлен в игры типа {gameType}");
		games1.Remove(gameId);
		await games.WriteStateAsync();
		ObserverManager.Notify(x => x.StateHasChanged());
	}
}