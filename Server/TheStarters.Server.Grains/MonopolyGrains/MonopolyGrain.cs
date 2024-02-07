using Mapster;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Core;
using Orleans.Runtime;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Abstractions.Monopoly;
using TheStarters.Server.Abstractions.Monopoly.Models;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Commands;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;
using TheStarters.Server.Grains.Consts;
using Throw;

namespace TheStarters.Server.Grains.MonopolyGrains;

public class MonopolyGrain(
	[PersistentState(stateName: "monopoly-game", storageName: StorageConsts.PersistenceStorage)]
	IStorage<MonopolyGame> game,
	[PersistentState(stateName: "monopoly-game-buy-requests", storageName: StorageConsts.PersistenceStorage)]
	IStorage<Dictionary<Guid, MonopolyBuyRequest>> buyRequests,
	ILogger<MonopolyGrain> logger)
	: ObservableGrain<IMonopolyObserver>(logger), IMonopolyGrain
{
	public ValueTask<MonopolyGame> GetAsync() => ValueTask.FromResult(game.State);

	public async Task<BaseGame> InitStateAsync(Guid userId)
	{
		var playerGrain = GrainFactory.GetGrain<IPlayerGrain>(userId);
		var player = await playerGrain.GetProfileAsync();
		
		var board = await GrainFactory.GetGrain<IMonopolyMetadataGrain>(Guid.Empty).ConfigureBoardAsync();
		var id = this.GetPrimaryKeyLong();
		game.State = MonopolyGame.InitState(id, player, board);
		await game.WriteStateAsync();
		await playerGrain.JoinGameAsync(game.State.GameType, game.State.Id);
		return game.State;
	}

	public async Task AddPlayerAsync(Guid userId)
	{
		if (game.State.Started)
			throw new GameStateException("Игра уже началась");
		if (game.State.Players.ContainsKey(userId))
			throw new GameStateException("Игрок уже добавлен в сессию");
		var playerGrain = GrainFactory.GetGrain<IPlayerGrain>(userId);
		var player = await playerGrain.GetProfileAsync();
		var monopolyPlayer = new MonopolyPlayer(player.Id, player.Name);
		game.State.Players.Add(userId, monopolyPlayer);
		await game.WriteStateAsync();
		await playerGrain.JoinGameAsync(game.State.GameType, game.State.Id);
	}

	public async Task RemovePlayerAsync(Guid userId)
	{
		game.State.Players.Remove(userId);
		await game.WriteStateAsync();
		var grain = GrainFactory.GetGrain<IPlayerGrain>(userId);
		await grain.RemoveFromGameAsync(GameType.Monopoly, game.State.Id);
		await NotifyAsync();
	}

	public async Task StartGameAsync(Guid userId)
	{
		if (game.State.Players.Keys.FirstOrDefault() != userId)
			throw new GameStateException("Игрок не является хоязином сессии");
		var gameScreen = game.State with { };
		try
		{
			if (game.State.Players.Count < 2)
				throw new GameStateException("Невозможно начать игру с одним игроком");
			game.State.ShufflePlayers();
			foreach (var player in game.State.Players.Values)
			{
				game.State.Board[1].OnLand(player);
			}
			game.State.CurrentPlayerId = game.State.Players.Values.First().Id;
			game.State.CurrentPlayerId.ThrowIfNull("Невозможно начать, т.к. недостаточно игроков.");
			game.State.Players[game.State.CurrentPlayerId.Value].AddCommand(new MoveCommand());
			game.State.Started = true;
			await game.WriteStateAsync();
			await NotifyAsync();
			logger.LogInformation($"Игра \"Монополия\" №{game.State.Id} начата.");
		}
		catch (Exception ex)
		{
			logger.LogError(ex.ToString());
			game.State = gameScreen;
			throw;
		}
	}

	public async Task ExecuteCommandAsync(Guid userId, int commandId)
	{
		var gameScreen = game.State with { };
		try
		{
			if (!game.State.Players.TryGetValue(userId, out var player))
				throw new GameStateException("Игрока нет в данной сессии");

			game.State.CurrentPlayerId.ThrowIfNull("Игра не началась.");
			game.State.CurrentPlayerId.Value.Throw("Ход другого игрока.").IfNotEquals(player.Id);
			if (!player.Commands.TryGetValue(commandId, out var command))
				throw new GameStateException($"У игрока нет команды с Id {commandId}");
			game.State.ExecuteCommand(command);
			await game.WriteStateAsync();
			await NotifyAsync();
			logger.LogInformation($"Игра: Монополия({game.State.Id}). Игрок выполнил действие: {command}");
		}
		catch (Exception ex)
		{
			logger.LogError(ex.ToString());
			game.State = gameScreen;
			throw;
		}
	}

	public ValueTask<Immutable<List<MonopolyBuyRequest>>> GetOwnerBuyRequestsAsync(Guid userId)
		=> ValueTask.FromResult(
			new Immutable<List<MonopolyBuyRequest>>(buyRequests.State.Values.Where(x => x.OwnerId == userId).ToList()));

	public async Task ConfirmBuyRequestAsync(Guid userId, Guid buyRequestId)
	{
		if (!buyRequests.State.TryGetValue(buyRequestId, out var buyRequest))
			throw new GameStateException("Запрос на покупку не найден");
		if (!game.State.Players.TryGetValue(userId, out var owner))
			throw new GameStateException("Игрок не найден");
		if (buyRequest.OwnerId != userId)
			throw new GameStateException("Подтверждать продажу может только хозяин поля");
		if (!game.State.Board.TryGetValue(buyRequest.LandId, out var land) || land is not IRentLand rentLand)
			throw new GameStateException("Поле не доступно к продаже");
		if (rentLand is StreetLand streetLand)
		{
			var cityStreets =
				game.State.Board.Values
					.Where(x => x is StreetLand sLand && sLand.City == streetLand.City)
					.OfType<StreetLand>()
					.ToArray();
			if (cityStreets.Any(x => x.HouseCount > 0 || x.HasHotel))
				throw new InvalidOperationException(
					"Перед продажей собственности другому игроку требуется, чтобы на всех улицах города не было домов и отелей");
		}

		if (!game.State.Players.TryGetValue(buyRequest.AuthorId, out var buyer))
			throw new GameStateException("Игрок-покупатель не найден");
		if (buyer.Balance < rentLand.SellCost)
			throw new GameStateException("У игрока-покупателя недостаточно средств");
		var screen = game.State with { };
		try
		{
			buyer.Balance -= rentLand.SellCost;
			owner.Balance += rentLand.SellCost;
			rentLand.OwnerId = buyer.Id;
			var toDeleteBuyRequests = buyRequests.State.Values
				.Where(x => x.LandId == buyRequest.LandId)
				.Select(x => x.Id);
			foreach (var deleteBuyRequestId in toDeleteBuyRequests)
			{
				buyRequests.State.Remove(deleteBuyRequestId);
			}

			await game.WriteStateAsync();
			await buyRequests.WriteStateAsync();
			await NotifyAsync();
			await ObserverManager.Notify(x => x.BuyRequestsChanged(userId));
		}
		catch (Exception e)
		{
			logger.LogError(e.ToString());
			game.State = screen;
			throw;
		}
	}

	public async Task CreateBuyLandRequestAsync(Guid userId, byte landId)
	{
		if (!game.State.Players.TryGetValue(userId, out var player))
			throw new GameStateException("Такого игрока нет в сессии");
		var land = game.State.Board.GetValueOrDefault(landId);
		if (land is null)
			throw new GameStateException($"Поле с Id:{land} не найдено");
		if (land is not IRentLand rentLand)
			throw new GameStateException("Поле не может быть куплено или продано");
		if (!rentLand.OwnerId.HasValue)
			throw new GameStateException("У поля нет хозяина");
		if (rentLand.OwnerId.Value == userId)
			throw new GameStateException("Игрок не может купить поле сам у себя");
		if (player.Balance < rentLand.SellCost)
			throw new GameStateException(
				$"У игрока недостаточно средств для покупки поля. Требуется: {rentLand.SellCost}");

		var monopolyBuyRequest = new MonopolyBuyRequest()
		{
			Id = Guid.NewGuid(),
			AuthorId = userId,
			OwnerId = rentLand.OwnerId.Value,
			CreatedOn = DateTime.UtcNow,
			LandId = landId
		};
		buyRequests.State.Add(monopolyBuyRequest.Id, monopolyBuyRequest);
		await buyRequests.WriteStateAsync();
		await ObserverManager.Notify(x => x.BuyRequestsChanged(rentLand.OwnerId.Value));
	}

	public async Task ExecuteLandOperationAsync(Guid userId, byte landId, LandOperation operation)
	{
		var screen = game.State with { };
		try
		{
			MonopolyCommand command = operation switch
			{
				LandOperation.Upgrade => new UpgradeStreetCommand(landId),
				LandOperation.Downgrade => new DowngradeStreetCommand(landId),
				LandOperation.Sell => new SellLandCommand(landId),
				_ => throw new NotImplementedException("Такая операция не предусмотрена")
			};
			game.State.ExecuteCommand(command, userId);
			await game.WriteStateAsync();
			await NotifyAsync();
		}
		catch (Exception ex)
		{
			logger.LogError(ex.ToString());
			game.State = screen;
			throw;
		}
	}

	public async Task EndTurnAsync(Guid userId)
	{
		var gameScreen = game.State with { };
		try
		{
			if (!game.State.Players.TryGetValue(userId, out var currentPlayer))
				throw new GameStateException("Игрока нет в данной сессии");

			game.State.CurrentPlayerId.ThrowIfNull("Игра не началась.");
			game.State.CurrentPlayerId.Value.Throw("Ход другого игрока.").IfNotEquals(currentPlayer.Id);
			if (currentPlayer.Commands.Any())
				throw new GameStateException("У пользователя есть невыполненные действия");

			//TODO AUCTION	
			currentPlayer.TurnDone = true;
			if (game.State.Players.All(x => x.Value.TurnDone))
			{
				foreach (var player in game.State.Players.Values)
				{
					player.TurnDone = false;
				}
			}

			game.State.CurrentPlayerId = game.State.Players.Values.First(x => !x.TurnDone).Id;
			currentPlayer = game.State.Players[game.State.CurrentPlayerId.Value];
			if (currentPlayer.Arrested)
			{
				currentPlayer.AddCommand(new PayForFreeCommand());
				currentPlayer.AddCommand(new ThrowDiceCommand());
			}
			else
			{
				currentPlayer.AddCommand(new MoveCommand());
			}

			await game.WriteStateAsync();
			await NotifyAsync();
			logger.LogInformation(
				$"Игра: Монополия({game.State.Id}). Игрок({userId}): завершил ход. Следующий ход игрока({game.State.CurrentPlayerId})");
		}
		catch (Exception ex)
		{
			logger.LogError(ex.ToString());
			game.State = gameScreen;
			throw;
		}
	}

	private ValueTask NotifyAsync()
	{
		ObserverManager.Notify(ob => ob.GameStateChanged());
		return ValueTask.CompletedTask;
	}
}