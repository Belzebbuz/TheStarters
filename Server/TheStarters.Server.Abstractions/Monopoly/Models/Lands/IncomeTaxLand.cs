using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Commands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Lands;

[GenerateSerializer, Immutable]
public sealed class IncomeTaxLand(byte id) : MonopolyLand(id)
{
	public short TaxValue => 200;

	public override void OnLand(MonopolyPlayer player)
	{
		AddPlayer(player);
		player.AddCommand(new PayIncomeTaxCommand(TaxValue));
	}
}