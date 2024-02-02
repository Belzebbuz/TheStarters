using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands;


[GenerateSerializer, Immutable]
public sealed class BuyStreetCommand
	: MonopolyCommand, IBuyCommand
{
	public override string Description => CommandDescriptions.BuyStreet;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		if (game.Board[currentPlayer.LandId] is not StreetLand companyLand)
			throw new GameStateException("Поле не является компанией.");
		if (companyLand.OwnerId is not null)
			throw new GameStateException("У компании уже есть хозяин");
		if (currentPlayer.Balance <= companyLand.Cost) return;
		
		companyLand.OwnerId = game.CurrentPlayerId;
		currentPlayer.Balance -= companyLand.Cost;
		currentPlayer.OwnedProperties.Add(currentPlayer.LandId);
		currentPlayer.RemoveCommand(Id);
	}
}

public interface IBuyCommand
{
}