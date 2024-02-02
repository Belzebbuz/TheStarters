using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands.CommunityChests;

[GenerateSerializer, Immutable]
public class FondChest : MonopolyCommand, ICommunityChest
{
	public override string Description => CommandDescriptions.Fond;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		currentPlayer.Balance += 100;
	}
}