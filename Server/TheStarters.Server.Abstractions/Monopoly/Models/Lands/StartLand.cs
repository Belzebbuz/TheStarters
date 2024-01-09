using TheStarters.Server.Abstractions.Monopoly.Models.Commands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Lands;

[GenerateSerializer, Immutable]
public sealed class StartLand(byte id) : MonopolyLand(id)
{
	public override void OnLand(MonopolyPlayer player)
	{
		AddPlayer(player);
		player.AddCommand(new MoveCommand( true));
	}
}