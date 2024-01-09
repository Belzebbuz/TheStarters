using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Server.Abstractions;

public interface IPlayerGrain : IGrainWithGuidKey
{
	ValueTask<PlayerProfile> GetProfileAsync();
	ValueTask UpdateProfileAsync(PlayerProfile playerProfile);
	ValueTask JoinGameAsync(GameType gameType, long gameId);
	ValueTask RemoveFromGameAsync(GameType gameType, long gameId);
}