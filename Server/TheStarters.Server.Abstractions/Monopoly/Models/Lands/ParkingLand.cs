using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Lands;

[GenerateSerializer, Immutable]
public sealed class ParkingLand(byte id) : MonopolyLand(id)
{
	public override void OnLand(MonopolyPlayer player)
	{
		AddPlayer(player);
	}
}