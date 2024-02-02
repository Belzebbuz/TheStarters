using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands.Chances;

[GenerateSerializer, Immutable]
public class GoToStationChance : MonopolyCommand, IChance
{
	public override string Description => CommandDescriptions.GoToStation;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		var lands = game.Board.Values
			.Where(x => x is TrainStationLand)
			.ToArray();
		var randomLands = Random.Shared.GetItems(lands, 1);
		randomLands[0].OnLand(currentPlayer);
	}
}