using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands;

public class UpgradeStreetCommand(byte landId) : MonopolyCommand
{
	public override string Description => CommandDescriptions.UpgradeStreet;
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

		if (cityStreets.Any(x => x.HasHotel) && streetLand.HouseCount == 4)
			throw new GameStateException("В городе можно построить только один отель");
		if (streetLand.HouseCount == 4)
		{
			if (currentPlayer.Balance < streetLand.HotelCost)
				throw new GameStateException("Недостаточно средств.");
			streetLand.HouseCount = 0;
			streetLand.HasHotel = true;
			currentPlayer.Balance -= streetLand.HotelCost;
		}
		else
		{
			if (currentPlayer.Balance < streetLand.HouseCost)
				throw new GameStateException("Недостаточно средств");

			streetLand.HouseCount += 1;
			if (cityStreets.Any(x => streetLand.HouseCount - x.HouseCount < 1))
				throw new GameStateException("Количество зданий в городе должно увеличиваться равномерно");
			currentPlayer.Balance -= streetLand.HouseCost;
		}
	}
}