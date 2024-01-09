using TheStarters.Server.Abstractions.Monopoly.Models.Lands;
using Throw;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands;

public sealed class PayRentCommand(bool required) : MonopolyCommand(required)
{
	public override void Execute(MonopolyGame game)
	{
		if (game.Board[game.CurrentPlayer!.LandId] is not CompanyLand companyLand)
			throw new InvalidOperationException("Поле не является полем компании.");
		if(companyLand.Owner is null)
			throw new InvalidOperationException("Компания не имеет хозяина для уплаты налога.");
		if (game.CurrentPlayer!.Id == companyLand.Owner.Id)
			throw new InvalidOperationException("Игрок является хозяином компании");
		if (game.CurrentPlayer.Balance < companyLand.Tax)
			throw new InvalidOperationException("У игрока недостаточно средств для оплаты аренды");
		game.CurrentPlayer.Balance -= companyLand.Tax;
		companyLand.Owner.Balance += companyLand.Tax;
		game.CurrentPlayer.RemoveCommand(Id);
	}
}