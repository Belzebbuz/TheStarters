using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Server.Abstractions;

public interface ITicTacToeGrain : IGrainWithIntegerKey
{
	ValueTask UpdateAsync(TicTacToeGame game);
	ValueTask AddPlayerAsync(Guid userId);
	ValueTask RemovePlayerAsync(Guid userId);
	ValueTask StartAsync();
	ValueTask<TicTacToeGame> GetAsync();
	ValueTask SetAnswerAsync(Guid userId, byte x, byte y);
}