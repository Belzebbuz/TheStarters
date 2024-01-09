using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Grains.Consts;
using Throw;

namespace TheStarters.Server.Grains;

public class PlayerGrain : IPlayerGrain
{
	private readonly IPersistentState<PlayerProfile> _player;
	private readonly IPersistentState<Dictionary<GameType,HashSet<long>>> _games;
	private readonly ILogger<PlayerGrain> _logger;

	public PlayerGrain(
		[PersistentState(stateName: "profile", storageName: StorageConsts.PersistenceStorage)]
		IPersistentState<PlayerProfile> player,
		[PersistentState(stateName: "player-games", storageName: StorageConsts.PersistenceStorage)]
		IPersistentState<Dictionary<GameType,HashSet<long>>> games,
		ILogger<PlayerGrain> logger)
	{
		_player = player;
		_games = games;
		_logger = logger;
	}

	public ValueTask<PlayerProfile> GetProfileAsync()
		=> ValueTask.FromResult(_player.State);

	public async ValueTask UpdateProfileAsync(PlayerProfile playerProfile)
	{
		if (playerProfile.Name is null)
			throw new ArgumentException("Пользователь должен указать имя");
		_player.State = playerProfile;
		await _player.WriteStateAsync();
	}

	public async ValueTask JoinGameAsync(GameType gameType, long gameId)
	{
		var games = _games.State.GetValueOrDefault(gameType);
		if (games is null)
		{
			games = new();
			_games.State[gameType] = games;
		}

		games.Add(gameId);
		await _games.WriteStateAsync();
	}

	public async ValueTask RemoveFromGameAsync(GameType gameType, long gameId)
	{
		var games = _games.State.GetValueOrDefault(gameType);
		games.ThrowIfNull($"Пользователь не был добавлен в игры типа {gameType}");
		games.Remove(gameId);
		await _games.WriteStateAsync();
	}
}