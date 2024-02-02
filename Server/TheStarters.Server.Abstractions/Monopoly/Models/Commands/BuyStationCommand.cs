using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands;

[GenerateSerializer, Immutable]
public class BuyStationCommand: MonopolyCommand, IBuyCommand
{
	public override string Description => CommandDescriptions.BuyStation;

	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		if (game.Board[currentPlayer.LandId] is not TrainStationLand stationLand)
			throw new GameStateException("Поле не является станцией.");
		if (stationLand.OwnerId is not null)
			throw new GameStateException("У компании уже есть хозяин");
		if (currentPlayer.Balance <= stationLand.Cost) return;
		
		stationLand.OwnerId = game.CurrentPlayerId;
		currentPlayer.Balance -= stationLand.Cost;
		currentPlayer.OwnedProperties.Add(currentPlayer.LandId);
		currentPlayer.RemoveCommand(Id);
		var ownedTrainStations =
			currentPlayer.OwnedProperties.Select(x => game.Board[x])
				.OfType<TrainStationLand>()
				.ToArray();
		foreach (var ownedStation in ownedTrainStations)
		{
			ownedStation.Tax = (short)(ownedTrainStations.Length * 25);
		}
	}
}