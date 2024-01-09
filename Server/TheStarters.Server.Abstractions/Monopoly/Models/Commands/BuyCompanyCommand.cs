using TheStarters.Server.Abstractions.Monopoly.Models.Lands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands;

[GenerateSerializer, Immutable]
public sealed class BuyCompanyCommand(bool required)
	: MonopolyCommand(required)
{
	public override void Execute(MonopolyGame game)
	{
		if (game.Board[game.CurrentPlayer!.LandId] is not CompanyLand companyLand) return;
		if (companyLand.Owner is not null)
			throw new InvalidOperationException("У компании уже есть хозяин");
		if (game.CurrentPlayer.Balance <= companyLand.Cost) return;
		
		companyLand.Owner = game.CurrentPlayer;
		game.CurrentPlayer.Balance -= companyLand.Cost;
		game.CurrentPlayer.RemoveCommand(Id);
	}
}