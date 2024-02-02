using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands.Chances;

public class GoToRandomStreetChance : MonopolyCommand, IChance
{
	public override string Description => CommandDescriptions.GoToRandomStreet;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		var lands = game.Board.Values
			.Where(x => x is StreetLand)
			.ToArray();
		var randomLands = Random.Shared.GetItems(lands, 1);
		randomLands[0].OnLand(currentPlayer);
	}
}