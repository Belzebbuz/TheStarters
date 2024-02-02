using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Lands;

[GenerateSerializer, Immutable]
public sealed class PrisonLand(byte id) : MonopolyLand(id)
{
	public override void OnLand(MonopolyPlayer player)
	{
		AddPlayer(player);
	}

	public void Arrest(MonopolyPlayer player)
	{
		player.Arrested = true;
		AddPlayer(player);
	}
}