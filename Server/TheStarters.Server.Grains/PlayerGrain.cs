using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Grains.Consts;

namespace TheStarters.Server.Grains;

public class PlayerGrain : IPlayerGrain
{
	private readonly IPersistentState<PlayerProfile> _player;
	private readonly IPersistentState<Dictionary<long,BaseGame>> _games;
	private readonly ILogger<PlayerGrain> _logger;

	public PlayerGrain(
		[PersistentState(stateName: "profile", storageName: StorageConsts.PersistenceStorage)]
		IPersistentState<PlayerProfile> player,
		[PersistentState(stateName: "player-games", storageName: StorageConsts.PersistenceStorage)]
		IPersistentState<Dictionary<long,BaseGame>> games,
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

	public async ValueTask AddOrUpdateGameAsync(BaseGame game)
	{
		_games.State[game.Id] = game;
		await _games.WriteStateAsync();
	}

	public async ValueTask RemoveFromGameAsync(BaseGame game)
	{
		if (!_games.State.Remove(game.Id, out var _))
		{
			_logger.LogWarning($"Пользователя уже нет в игре №{game.Id}");
		}
		await _games.WriteStateAsync();
	}
}