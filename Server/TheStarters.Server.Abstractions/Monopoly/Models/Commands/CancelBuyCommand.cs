using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands;

[GenerateSerializer, Immutable]
public class CancelBuyCommand : MonopolyCommand
{
	public override string Description => CommandDescriptions.CancelBuy;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		var buyCommands = currentPlayer.Commands
			.Where(x => x.Value is IBuyCommand)
			.Select(x => x.Key);
		foreach (var buyCommand in buyCommands)
		{
			currentPlayer.RemoveCommand(buyCommand);
		}

		game.AuctionStarted = true;
		currentPlayer.RemoveCommand(Id);
	}
}