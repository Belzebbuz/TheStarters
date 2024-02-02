using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands.CommunityChests;

[GenerateSerializer, Immutable]
public class HospitalizationExpensesChest : MonopolyCommand, ICommunityChest
{
	public override string Description => CommandDescriptions.HospitalizationExpenses;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		if (currentPlayer.Balance < 100)
			throw new GameStateException("У игрока не достаточно средств. Требуется 100");
		currentPlayer.Balance -= 100;
	}
}