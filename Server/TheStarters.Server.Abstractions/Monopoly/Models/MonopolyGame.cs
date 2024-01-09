using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Abstractions.Monopoly.Models.Commands;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;
using Throw;

namespace TheStarters.Server.Abstractions.Monopoly.Models;

[GenerateSerializer, Immutable]
public sealed record MonopolyGame : BaseGame
{
	[Id(0)] public List<MonopolyPlayer> Players { get; set; } = new (8);
	[Id(1)] public MonopolyPlayer? CurrentPlayer { get; set; }
	[Id(2)] public required Dictionary<byte,MonopolyLand> Board { get; init; } = [];
	[Id(3)] public DicePair DicePair { get; } = new();
	public void ExecuteCommand(MonopolyCommand command)
	{
		CurrentPlayer.ThrowIfNull("Текущий игрок не установлен");
		var existCommand = CurrentPlayer.Commands[command.Id];
		existCommand.ThrowIfNull($"Игрок {CurrentPlayer.Name} не обладает таким действием");
		existCommand.Execute(this);
	}

	public void ShufflePlayers()
	{
		var players = Players.ToArray();
		Random.Shared.Shuffle(players);
		Players = players.ToList();
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
			GameType = GameType.Monopoly
		};
		game.Players.Add(new MonopolyPlayer(player.Id,player.Name ?? "N/A"));
		return game;
	}
}