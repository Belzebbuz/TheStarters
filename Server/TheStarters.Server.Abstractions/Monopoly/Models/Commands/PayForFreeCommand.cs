using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands;

[GenerateSerializer, Immutable]
public class PayForFreeCommand : MonopolyCommand
{
	public override string Description => CommandDescriptions.PayForFree;
	public short Value => 50;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		if (!currentPlayer.Arrested)
			throw new GameStateException("Игрок не арестован");
		if (currentPlayer.Balance < Value)
			throw new GameStateException("Недостаточно средств");
		currentPlayer.Balance -= Value;
		currentPlayer.Arrested = false;
		currentPlayer.Commands.Clear();
		currentPlayer.AddCommand(new MoveCommand());
	}
}