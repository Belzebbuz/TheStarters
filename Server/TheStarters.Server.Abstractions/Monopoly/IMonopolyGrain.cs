using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Abstractions.Monopoly.Models;
using TheStarters.Server.Abstractions.Monopoly.Models.Commands;

namespace TheStarters.Server.Abstractions.Monopoly;

public interface IMonopolyGrain : IGrainWithIntegerKey, IObservableGrain<IMonopolyObserver>
{
	ValueTask<BaseGame> InitStateAsync(Guid userId);
	ValueTask<MonopolyGame> GetAsync();
	ValueTask StartGameAsync();
	ValueTask ExecuteCommandAsync<T>(Guid userId, T command) where T: MonopolyCommand;
	ValueTask EndTurnAsync(Guid userId);
}

public interface IMonopolyObserver : IGrainObserver
{
	ValueTask GameStateChanged();
}