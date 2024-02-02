﻿using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands.CommunityChests;

[GenerateSerializer, Immutable]
public class GoToGoLandChest : MonopolyCommand, ICommunityChest
{
	public override string Description => CommandDescriptions.GoToGoLand;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		var goLand = game.Board.Values.FirstOrDefault(x => x is GoLand) ?? throw new GameStateException("Не найдено поле ВПЕРЕД");
		goLand.OnLand(currentPlayer);
		currentPlayer.Balance += 200;
	}
}