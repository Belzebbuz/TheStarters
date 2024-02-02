using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Server.Abstractions;

public interface IPlayerGrain : IGrainWithGuidKey
{
	ValueTask<PlayerProfile> GetProfileAsync();
	Task SetNameAsync(string name);
	Task JoinGameAsync(GameType gameType, long gameId);
	Task RemoveFromGameAsync(GameType gameType, long gameId);
}

public interface IPlayerObserver : IGrainObserver
{
	public ValueTask StateHasChanged();
}