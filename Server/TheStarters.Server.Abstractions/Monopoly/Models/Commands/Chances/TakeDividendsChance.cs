using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands.Chances;

[GenerateSerializer, Immutable]
public class TakeDividendsChance : MonopolyCommand, IChance
{
	public override string Description => CommandDescriptions.TakeDividends;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		currentPlayer.Balance += 50;
	}
}