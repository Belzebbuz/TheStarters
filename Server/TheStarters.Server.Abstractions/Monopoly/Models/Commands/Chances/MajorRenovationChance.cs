using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands.Chances;

[GenerateSerializer, Immutable]
public class MajorRenovationChance : MonopolyCommand, IChance
{
	public override string Description => CommandDescriptions.MajorRenovation;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		var lands = game.Board.Values
			.Where(x => x is StreetLand streetLand && streetLand.OwnerId == currentPlayer.Id)
			.OfType<StreetLand>()
			.ToArray();
		var totalHouseTax = lands.Sum(x => x.HouseCount) * 25;
		var totalHotelTax = lands.Count(x => x.HasHotel) * 100;
		var total = totalHouseTax + totalHotelTax;
		if (currentPlayer.Balance < total)
			throw new GameStateException($"У игрока недостаточно средств. Требуется: {total}");
		currentPlayer.Balance -= total;
	}
}