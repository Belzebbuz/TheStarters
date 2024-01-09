namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands;

public class SellOwnedCommand(bool required) : MonopolyCommand(required)
{
	public override void Execute(MonopolyGame game)
	{
		throw new NotImplementedException();
	}
}