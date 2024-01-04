using Orleans.Runtime;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Grains.Consts;

namespace TheStarters.Server.Grains;

public class PlayerGrain : IPlayerGrain
{
	private readonly IPersistentState<PlayerProfile> _player;
	private readonly IPersistentState<HashSet<long>> _games;

	public PlayerGrain(
		[PersistentState(stateName: "profile", storageName: StorageConsts.PersistenceStorage)]
		IPersistentState<PlayerProfile> player,
		[PersistentState(stateName: "player-games", storageName: StorageConsts.PersistenceStorage)]
		IPersistentState<HashSet<long>> games)
	{
		_player = player;
		_games = games;
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

	public async ValueTask AddToGameAsync(long gameId)
	{
		_games.State.Add(gameId);
		await _games.WriteStateAsync();
	}

	public async ValueTask RemoveFromGameAsync(long gameId)
	{
		_games.State.Remove(gameId);
		await _games.WriteStateAsync();
	}
}