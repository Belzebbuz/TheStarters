using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Abstractions.Monopoly;
using TheStarters.Server.Abstractions.Monopoly.Models;
using TheStarters.Server.Abstractions.Monopoly.Models.Commands;
using TheStarters.Server.Grains.Consts;
using Throw;

namespace TheStarters.Server.Grains.MonopolyGrains;

public class MonopolyGrain(
	[PersistentState(stateName: "monopoly-game", storageName: StorageConsts.PersistenceStorage)]
	IPersistentState<MonopolyGame> game,
	ILogger<MonopolyGrain> logger)
	: ObservableGrain<IMonopolyObserver>(logger), IMonopolyGrain
{
	public ValueTask<MonopolyGame> GetAsync() => ValueTask.FromResult(game.State);
	public async ValueTask<BaseGame> InitStateAsync(Guid userId)
	{
		var playerGrain = GrainFactory.GetGrain<IPlayerGrain>(userId);
		var player = await playerGrain.GetProfileAsync();
		var board = await GrainFactory.GetGrain<IMonopolyMetadataGrain>(Guid.Empty).ConfigureBoardAsync();
		var id = this.GetPrimaryKeyLong();
		game.State = MonopolyGame.InitState(id, player, board);
		await game.WriteStateAsync();
		await playerGrain.JoinGameAsync(game.State.GameType, game.State.Id);
		await GrainFactory.GetGrain<IGamesGrain>(Guid.Empty).AddOrUpdateGameAsync(game.State);
		return game.State;
	}

	public async ValueTask StartGameAsync()
	{
		// if (game.State.Players.Count < 2)
		// 	throw new InvalidOperationException("Невозможно начать игру с одним игроком");
		game.State.Players.Add(new MonopolyPlayer(Guid.Empty, Faker.Name.First()));
		game.State.ShufflePlayers();
		foreach (var player in game.State.Players)
		{
			game.State.Board[1].OnLand(player);
		}
		game.State.CurrentPlayer = game.State.Players.FirstOrDefault();
		game.State.CurrentPlayer.ThrowIfNull("Невозможно начать, т.к. недостаточно игроков.");
		
		game.State.Started = true;
		await game.WriteStateAsync();
		await NotifyAsync();
		logger.LogInformation($"Игра \"Монополия\" №{game.State.Id} начата.");
	}

	public async ValueTask ExecuteCommandAsync<T>(Guid userId, T command) 
		where T : MonopolyCommand
	{
		game.State.CurrentPlayer.ThrowIfNull("Игра не началась.");
		game.State.CurrentPlayer.Id.Throw("Ход другого игрока.").IfNotEquals(userId);
		game.State.ExecuteCommand(command);
		await game.WriteStateAsync();
		await NotifyAsync();
		logger.LogInformation($"Игра: Монополия({game.State.Id}). Игрок выполнил действие: {command}");
	}

	public async ValueTask EndTurnAsync(Guid userId)
	{
		game.State.CurrentPlayer.ThrowIfNull("Игра не началась.");
		game.State.CurrentPlayer.Id.Throw("Ход другого игрока.").IfNotEquals(userId);
		if (game.State.CurrentPlayer.Commands.Any(command => command.Value.Required))
			throw new InvalidOperationException("У пользователя есть невыполненные обязательные действия");

		game.State.CurrentPlayer.TurnDone = true;
		if (game.State.Players.All(x => x.TurnDone)) 
			game.State.Players.ForEach(x => x.TurnDone = false);
		
		game.State.CurrentPlayer = game.State.Players.First(x => !x.TurnDone);
		
		if (!game.State.CurrentPlayer.Commands.Values.OfType<MoveCommand>().Any()) 
			game.State.CurrentPlayer.AddCommand(new MoveCommand(true));
		
		await game.WriteStateAsync();
		await NotifyAsync();
		logger.LogInformation($"Игра: Монополия({game.State.Id}). Игрок({userId}): завершил ход. Следующий ход игрока({game.State.CurrentPlayer.Id})");
	}

	private ValueTask NotifyAsync()
	{
		ObserverManager.Notify(ob => ob.GameStateChanged());
		return ValueTask.CompletedTask;
	}
}