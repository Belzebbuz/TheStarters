using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;
using Throw;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands;

[GenerateSerializer, Immutable]
public sealed class PayRentCommand(short value) : MonopolyCommand
{
	public override string Description { get; } = CommandDescriptions.PayRent(value);
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		if (game.Board[currentPlayer.LandId] is not IRentLand taxLand)
			throw new GameStateException("Поле не является полем компании.");
		if(taxLand.OwnerId is null)
			throw new GameStateException("Компания не имеет хозяина для уплаты налога.");
		if (currentPlayer.Id == taxLand.OwnerId)
			throw new GameStateException("Игрок является хозяином компании");
		if (currentPlayer.Balance < taxLand.Tax)
			throw new GameStateException("У игрока недостаточно средств для оплаты аренды");
		var owner = game.Players[taxLand.OwnerId.Value];
		currentPlayer.Balance -= taxLand.Tax;
		owner.Balance += taxLand.Tax;
		currentPlayer.RemoveCommand(Id);
	}
}