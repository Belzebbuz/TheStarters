namespace TheStarters.Server.Abstractions.Monopoly.Models.Lands;

[GenerateSerializer, Immutable]
public sealed class ParkingMonopolyLand(byte id) : MonopolyLand(id)
{
	public override void OnLand(MonopolyPlayer player)
	{
		throw new NotImplementedException();
	}
}