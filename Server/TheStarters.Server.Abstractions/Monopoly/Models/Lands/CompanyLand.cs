using TheStarters.Server.Abstractions.Monopoly.Models.Commands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Lands;

[GenerateSerializer, Immutable]
public sealed class CompanyLand(byte id, string name, ushort cost, string street) : MonopolyLand(id)
{
	[Id(0)] public string Name { get; } = name;
	[Id(1)] public ushort Cost { get; } = cost;
	[Id(2)] public string Street { get; } = street;
	[Id(3)] public MonopolyPlayer? Owner { get; set; }
	[Id(4)] public decimal Tax { get; set; } = cost * 0.1m;

	public override void OnLand(MonopolyPlayer player)
	{
		AddPlayer(player);
		if (Owner is null)
			player.AddCommand(new BuyCompanyCommand(false));
		if (Owner is not null && Owner.Id != player.Id)
		{
			if (player.Balance > Tax)
				player.AddCommand(new PayRentCommand(true));
		}
	}
}