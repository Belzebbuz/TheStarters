using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Grains.Consts;
using Throw;

namespace TheStarters.Server.Grains;

public class TicTacToeGrain : ObservableGrain<ITicTacToeObserver>, ITicTacToeGrain
{
	private readonly ILogger<TicTacToeGrain> _logger;
	private readonly IPersistentState<TicTacToeGame> _game;
	public TicTacToeGrain(
		[PersistentState(stateName:"ticTacToe",storageName:StorageConsts.PersistenceStorage)] 
		IPersistentState<TicTacToeGame> game, 
		ILogger<TicTacToeGrain> logger)
	:base(logger)
	{
		_game = game;
		_logger = logger;
	}

	public async ValueTask<TicTacToeGame> InitStateAsync(Guid userId)
	{
		var id = this.GetPrimaryKeyLong();
		_game.State = TicTacToeGame.InitState(id, userId);
		await _game.WriteStateAsync();
		await NotifyAsync();
		_logger.LogInformation($"Игра \"крестики нолики\" №{id} создана.");
		return _game.State;
	}

	public async ValueTask AddPlayerAsync(Guid userId)
	{
		if (_game.State.Player1 == userId || _game.State.Player2 == userId)
			throw new InvalidOperationException("Игрок уже добавлен");
		if (_game.State.Player2.HasValue)
			throw new InvalidOperationException("Все места заняты");
		_game.State.SetSecondPlayer(userId);
		_game.State.Update();
		await _game.WriteStateAsync();
		
		await GrainFactory.GetGrain<IPlayerGrain>(userId).JoinGameAsync(_game.State.GameType, _game.State.Id);
		await GrainFactory.GetGrain<IGamesGrain>(Guid.Empty).AddOrUpdateGameAsync(_game.State);
		await NotifyAsync();
	}

	public async ValueTask RemovePlayerAsync(Guid userId)
	{
		_game.State.Started.Throw("Игра еще уже начата").IfTrue();
		_game.State.Player1.Throw("Невозможно удалить создателя сессии").IfEquals(userId);
		_game.State.Player2.ThrowIfNull("Такой пользователь отсутствует в сессии").IfNotEquals(userId);
		_game.State.SetSecondPlayer(null);
		_game.State.Update();
		await _game.WriteStateAsync();
		
		await GrainFactory.GetGrain<IPlayerGrain>(userId).RemoveFromGameAsync(_game.State.GameType, _game.State.Id);
		await GrainFactory.GetGrain<IGamesGrain>(Guid.Empty).AddOrUpdateGameAsync(_game.State);
		
		await NotifyAsync();
	}

	public async ValueTask StartAsync()
	{
		_game.State.Started.Throw("Игра еще уже начата").IfTrue();
		_game.State.Player2.HasValue.Throw("Необходим второй игрок").IfFalse();
		_game.State.Started = true;
		_game.State.CurrentPlayer = _game.State.Player1;
		await _game.WriteStateAsync();

		await GrainFactory.GetGrain<IGamesGrain>(Guid.Empty).AddOrUpdateGameAsync(_game.State);
		await NotifyAsync();
	}

	public ValueTask<TicTacToeGame> GetAsync() 
		=> ValueTask.FromResult(_game.State);

	public async ValueTask SetAnswerAsync(Guid userId, byte x, byte y)
	{
		_game.State.Started.Throw("Игра еще не начата").IfFalse();
		_game.State.Winner.HasValue.Throw("Игра уже закончена").IfTrue();
		_game.State.CurrentPlayer.Throw("Ход другого игрока").IfNotEquals(userId);
		_game.State.Board[x, y].HasValue.Throw("Поле уже имеет значение").IfTrue();
		_game.State.Board[x, y] = userId;
		var winner = _game.State.GetWinner();
		_game.State.Update();
		if (winner is not null)
		{
			_game.State.Winner = winner;
		}
		else
		{
			_game.State.CurrentPlayer = _game.State.CurrentPlayer == _game.State.Player1
				? _game.State.Player2!.Value
				: _game.State.Player1;
		}

		await _game.WriteStateAsync();
		await NotifyAsync();
	}

	public ValueTask NotifyAsync()
	{
		ObserverManager.Notify(ob => ob.GameStateChanged());
		return ValueTask.CompletedTask;
	}
}