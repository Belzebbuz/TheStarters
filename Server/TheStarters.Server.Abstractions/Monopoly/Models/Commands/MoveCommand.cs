using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands;

[GenerateSerializer, Immutable]
public sealed class MoveCommand
	: MonopolyCommand
{
	public override string Description => CommandDescriptions.Move;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		if (currentPlayer.Commands.Any(x => x.Value is not MoveCommand))
			throw new GameStateException("Перед началом хода игрок должен потратить другие действия");
			
		game.DicePair.Next();
		var next = (byte)((game.DicePair.Total + currentPlayer.LandId) % game.Board.LastOrDefault().Key);
		if (next < game.DicePair.Total + currentPlayer.LandId)
			currentPlayer.Balance += 200;
		game.Board[next].OnLand(currentPlayer);
		currentPlayer.RemoveCommand(Id);
		if (game.DicePair.First == game.DicePair.Second)
		{
			currentPlayer.TurnDiceDoubles++;
			if(currentPlayer.TurnDiceDoubles == 3)
				currentPlayer.AddCommand(new GoToPrisonCommand());
			else
				currentPlayer.AddCommand(new MoveCommand());
		}
		else
		{
			currentPlayer.TurnDiceDoubles = 0;
		}
	}
}