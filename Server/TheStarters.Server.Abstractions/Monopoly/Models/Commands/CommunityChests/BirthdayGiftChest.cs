using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands.CommunityChests;

[GenerateSerializer, Immutable]
public class BirthdayGiftChest : MonopolyCommand, ICommunityChest
{
	public override string Description => CommandDescriptions.BirthdayGift;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		foreach (var player in game.Players.Where(x => x.Key != currentPlayer.Id))
		{
			if (player.Value.Balance < 10) continue;
			
			player.Value.Balance -= 10;
			currentPlayer.Balance += 10;
		}
	}
}