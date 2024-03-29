using org.apache.zookeeper.data;

namespace TheStarters.Server.Abstractions.Models;

[GenerateSerializer, Immutable]
public record TicTacToeGame : BaseGame
{
	[Id(0)]
	public Guid Player1 { get; set;	}
	
	[Id(1)]
	public Guid? Player2 { get; set; }
	
	[Id(2)]
	public required Guid?[,] Board { get; init; }
	
	[Id(3)]
	public Guid? CurrentPlayer { get; set; }

	public void SetSecondPlayer(Guid? id)
	{
		Player2 = id;
		PlayersCount = (byte)(id.HasValue ? 2 : 1);
	}
	
	public Guid? GetWinner()
	{
		for (var row = 0; row < 3; row++)
		{
			if (Board[row, 0].HasValue && Board[row, 0] == Board[row, 1] && Board[row, 1] == Board[row, 2])
				return Board[row, 0]!.Value;
		}

		for (var col = 0; col < 3; col++)
		{
			if (Board[0, col].HasValue && Board[0, col] == Board[1, col] && Board[1, col] == Board[2, col])
				return Board[0, col]!.Value;
		}

		if (Board[0, 0].HasValue && Board[0, 0] == Board[1, 1] && Board[1, 1] == Board[2, 2])
			return Board[0, 0]!.Value;

		if (Board[0, 2].HasValue && Board[0, 2] == Board[1, 1] && Board[1, 1] == Board[2, 0])
			return Board[0, 2]!.Value;

		return null;
	}

	public static TicTacToeGame InitState(long id, Guid userId)
	{
		return new()
		{
			Id = id,
			Player1 = userId,
			GameType = GameType.TicTacToe,
			CreatedOn = DateTime.UtcNow,
			LastUpdateOn = DateTime.UtcNow,
			Board = new Guid?[3,3]
		};
	}
}