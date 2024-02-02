using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands.Chances;

[GenerateSerializer, Immutable]
public class GoToPrisonChance: MonopolyCommand, IChance
{
	public override string Description => CommandDescriptions.GoToPrison;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		var prison = game.Board.Values.FirstOrDefault(x => x is PrisonLand) as PrisonLand
		             ?? throw new GameStateException("Поле ТЮРЬМА не найдено");
		prison.Arrest(currentPlayer);
	}
}