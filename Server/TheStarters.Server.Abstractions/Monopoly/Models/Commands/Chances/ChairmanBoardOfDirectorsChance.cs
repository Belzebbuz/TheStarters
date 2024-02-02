using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands.Chances;

[GenerateSerializer, Immutable]
public class ChairmanBoardOfDirectorsChance : MonopolyCommand, IChance
{
	public override string Description => CommandDescriptions.ChairmanBoardOfDirectors;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		var total = game.Players.Count(x => x.Key != currentPlayer.Id) * 50;
		if (currentPlayer.Balance < total)
			throw new GameStateException($"У игрока недостаточно средств. Требуется оплатить: {total}");
		currentPlayer.Balance -= total;
		foreach (var player in game.Players.Where(x => x.Key != currentPlayer.Id))
		{
			player.Value.Balance += 50;
		}
	}
}