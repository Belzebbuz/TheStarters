using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands;

public class DowngradeStreetCommand(byte landId) : MonopolyCommand
{
	public override string Description => CommandDescriptions.DowngradeStreet;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		if (!game.Board.TryGetValue(landId, out var land) || land is not StreetLand streetLand)
			throw new GameStateException("Поле не найдено или не является улицей.");
		if (!currentPlayer.OwnedProperties.Contains(streetLand.Id))
			throw new GameStateException("Эта улица не яляется собственностью игрока");
		var cityStreets =
			game.Board.Values
				.Where(x => x is StreetLand sLand && sLand.City == streetLand.City)
				.OfType<StreetLand>()
				.ToArray();
		if (cityStreets.Any(x => x.OwnerId != currentPlayer.Id))
			throw new GameStateException("У игрока в собственности должны быть все улицы города.");
		if (streetLand.HasHotel)
		{
			streetLand.HasHotel = false;
			streetLand.HouseCount = 4;
			currentPlayer.Balance += streetLand.HotelCost / 2m;
		}
		else
		{
			streetLand.HouseCount -= 1;
			if (cityStreets.Any(x => streetLand.HouseCount - x.HouseCount < 1))
				throw new GameStateException("Количество зданий в городе должно увеличиваться равномерно");
			currentPlayer.Balance += streetLand.HouseCost / 2m;
		}
	}
}