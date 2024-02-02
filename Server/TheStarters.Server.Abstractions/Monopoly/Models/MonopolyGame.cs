using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using Throw;

namespace TheStarters.Server.Abstractions.Monopoly.Models;

[GenerateSerializer, Immutable]
public sealed record MonopolyGame : BaseGame
{
	[Id(0)] public IDictionary<Guid,MonopolyPlayer> Players { get; set; } = new Dictionary<Guid, MonopolyPlayer>(8);
	[Id(1)] public Guid? CurrentPlayerId { get; set; }
	[Id(2)] public required Dictionary<byte,MonopolyLand> Board { get; init; } = [];
	[Id(3)] public DicePair DicePair { get; } = new();
	[Id(4)] public bool AuctionStarted { get; set; }

	private MonopolyGame()
	{
	}
	public void ExecuteCommand(MonopolyCommand command)
	{
		CurrentPlayerId.ThrowIfNull("Текущий игрок не установлен");
		var currentPlayer = Players[CurrentPlayerId.Value];
		var existCommand = currentPlayer.Commands[command.Id];
		existCommand.ThrowIfNull($"Игрок {currentPlayer.Name} не обладает таким действием");
		existCommand.Execute(this,currentPlayer);
	}
	public void ExecuteCommand(MonopolyCommand command, Guid playerId)
	{
		if (!Players.TryGetValue(playerId, out var player))
			throw new GameStateException("Пользователя нет в данной сессии.");
		var existCommand = player.Commands[command.Id];
		existCommand.Execute(this,player);
	}
	public void ShufflePlayers()
	{
		var players = Players.Values.ToArray();
		Random.Shared.Shuffle(players);
		Players = players.ToDictionary(x => x.Id, x => x);
	}
	public static MonopolyGame InitState(long id, PlayerProfile player, Dictionary<byte,MonopolyLand> board)
	{
		var game = new MonopolyGame()
		{
			Id = id,
			Board = board,
			CreatedOn = DateTime.UtcNow,
			LastUpdateOn = DateTime.UtcNow,
			PlayersCount = 1,
			GameType = GameType.Monopoly,
		};
		game.Players.Add( player.Id, new MonopolyPlayer(player.Id,player.Name));
		return game;
	}
}