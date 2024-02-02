using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands.CommunityChests;

[GenerateSerializer, Immutable]
public class InheritanceChest : MonopolyCommand, ICommunityChest
{
	public override string Description => CommandDescriptions.Inheritance;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		currentPlayer.Balance += 100;
	}
}