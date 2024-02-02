using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands;

[GenerateSerializer, Immutable]
public class ThrowDiceCommand : MonopolyCommand
{
	public override string Description => CommandDescriptions.ThrowDice;

	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		game.DicePair.Next();
		if (!currentPlayer.Arrested || game.DicePair.First != game.DicePair.Second) return;
		
		currentPlayer.Arrested = false;
		currentPlayer.Commands.Clear();
		currentPlayer.AddCommand(new MoveCommand());
	}
}