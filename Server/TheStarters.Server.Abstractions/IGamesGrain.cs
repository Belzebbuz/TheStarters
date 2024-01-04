using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Server.Abstractions;

public interface IGamesGrain : IGrainWithGuidKey
{
	ValueTask<List<BaseGame>> GetCreatedAsync();
	ValueTask AddGameAsync(BaseGame game);
}