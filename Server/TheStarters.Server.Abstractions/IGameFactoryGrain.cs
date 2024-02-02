using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Server.Abstractions;

public interface IGameFactoryGrain : IGrainWithGuidKey, IObservableGrain<IGameFactoryObserver>
{
	ValueTask<long> CreateGameAsync(GameType gameType, Guid userId);
}

public interface IGameFactoryObserver : IGrainObserver
{
	Task GameCreated(GameType gameType, long id);
}