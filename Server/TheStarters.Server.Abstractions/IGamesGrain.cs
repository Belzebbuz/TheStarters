using Orleans.Concurrency;
using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Server.Abstractions;

public interface IGamesGrain : IGrainWithGuidKey
{
	ValueTask<Immutable<ICollection<BaseGame>>> GetCreatedAsync();
	ValueTask AddOrUpdateGameAsync(BaseGame game);
	ValueTask<Immutable<ICollection<BaseGame>>> GetLastUpdatedAsync();
}
