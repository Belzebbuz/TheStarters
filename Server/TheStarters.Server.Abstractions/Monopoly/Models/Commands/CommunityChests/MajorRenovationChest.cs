using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands.CommunityChests;

[GenerateSerializer, Immutable]
public class MajorRenovationChest : MonopolyCommand, ICommunityChest
{
	public override string Description => CommandDescriptions.MajorRenovationChest;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		var lands = game.Board.Values
			.Where(x => x is StreetLand streetLand && streetLand.OwnerId == currentPlayer.Id)
			.OfType<StreetLand>()
			.ToArray();
		var totalHouseTax = lands.Sum(x => x.HouseCount) * 40;
		var totalHotelTax = lands.Count(x => x.HasHotel) * 115;
		var total = totalHouseTax + totalHotelTax;
		if (currentPlayer.Balance < total)
			throw new GameStateException($"У игрока недостаточно средств. Требуется: {total}");
		currentPlayer.Balance -= total;
	}
}