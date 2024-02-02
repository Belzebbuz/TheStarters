using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands.Chances;

[GenerateSerializer,Immutable]
public class SpeedingFineChance : MonopolyCommand, IChance
{
	public override string Description => CommandDescriptions.SpeedingFine;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		if (currentPlayer.Balance < 15)
			throw new GameStateException("У игрока недостаточно средств. Требуется 15");
		currentPlayer.Balance -= 15;
	}
}