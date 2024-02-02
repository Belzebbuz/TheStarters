using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands.CommunityChests;

[GenerateSerializer, Immutable]
public class DoctorVisitChest : MonopolyCommand, ICommunityChest
{
	public override string Description => CommandDescriptions.DoctorVisit;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		if (currentPlayer.Balance < 50)
			throw new GameStateException("У игрока не достаточно средств. Требуется 50");
		currentPlayer.Balance -= 50;
	}
}