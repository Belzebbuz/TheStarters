using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Server.Abstractions;

public interface ITicTacToeGrain : IGrainWithIntegerKey, IObservableGrain<ITicTacToeObserver>
{
	ValueTask<TicTacToeGame> InitStateAsync(Guid userId);
	ValueTask AddPlayerAsync(Guid userId);
	ValueTask RemovePlayerAsync(Guid userId);
	ValueTask StartAsync(Guid userId);
	ValueTask<TicTacToeGame> GetAsync();
	ValueTask SetAnswerAsync(Guid userId, byte x, byte y);
}


public interface ITicTacToeObserver : IGrainObserver
{
	ValueTask GameStateChanged();
}