using Orleans.Concurrency;
using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Server.Abstractions;

public interface IGamesListGrain : IGrainWithGuidKey, IObservableGrain<IGamesListObserver>
{
	ValueTask<Immutable<ICollection<BaseGame>>> GetAsync(int page, int pageSize);
	ValueTask AddOrUpdateGameAsync(BaseGame game);
	ValueTask<Immutable<ICollection<BaseGame>>> GetLastUpdatedAsync();
}

public interface IGamesListObserver: IGrainObserver
{
	Task ListStateChanged();
}