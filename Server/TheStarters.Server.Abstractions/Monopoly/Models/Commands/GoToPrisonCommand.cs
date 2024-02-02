using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands;

[GenerateSerializer, Immutable]
public class GoToPrisonCommand : MonopolyCommand
{
	public override string Description => CommandDescriptions.GoToPrison;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		var land = game.Board.FirstOrDefault(x => x.Value is PrisonLand).Value;
		if (land is not PrisonLand prisonLand)
			throw new GameStateException("Поле тюрьмы не добавлено на доску");
		prisonLand.Arrest(currentPlayer);
		currentPlayer.Commands.Clear();
	}
}