namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands;

[GenerateSerializer, Immutable]
public sealed class MoveCommand(bool required) 
	: MonopolyCommand(required)
{
	public override void Execute(MonopolyGame game)
	{
		game.DicePair.Next();
		var next = (byte)((game.DicePair.Total + game.CurrentPlayer!.LandId) % game.Board.LastOrDefault().Key);
		game.Board[game.CurrentPlayer.LandId].CurrentPlayers.Remove(game.CurrentPlayer!.Id, out var _);
		game.Board[next].OnLand(game.CurrentPlayer);
		game.CurrentPlayer.RemoveCommand(Id);
		if(game.DicePair.First == game.DicePair.Second)
			game.CurrentPlayer.AddCommand(new MoveCommand(true));
	}
}