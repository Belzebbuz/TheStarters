using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands;

public class SellLandCommand(byte landId) : MonopolyCommand
{
	public override string Description => CommandDescriptions.SellLand;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		if (!game.Board.TryGetValue(landId, out var land))
			throw new GameStateException("Собственности с таким Id не существует");
			
		if (currentPlayer.OwnedProperties.Any(x => x != land.Id))
			throw new GameStateException("У игрока нет такой собственности");

		switch (land)
		{
			case StreetLand { HouseCount: > 0 }:
				throw new GameStateException("Невозможно продать улицу с домами");
			case StreetLand { HasHotel: true }:
				throw new GameStateException("Невозможно продать улицу отелем");
			case StreetLand streetLand:
				streetLand.OwnerId = null;
				currentPlayer.OwnedProperties.Remove(land.Id);
				currentPlayer.Balance += streetLand.SellCost;
				break;
			case TrainStationLand stationLand:
			{
				stationLand.OwnerId = null;
				stationLand.Tax = 25;
				currentPlayer.OwnedProperties.Remove(land.Id);
				currentPlayer.Balance += stationLand.SellCost;
				var ownedTrainStations =
					currentPlayer.OwnedProperties.Select(x => game.Board[x])
						.OfType<TrainStationLand>()
						.ToArray();
				foreach (var ownedStation in ownedTrainStations)
				{
					ownedStation.Tax = (short)(ownedTrainStations.Length * 25);
				}
				break;
			}
		}
	}
}