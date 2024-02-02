using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Commands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Lands;

[GenerateSerializer, Immutable]
public class TrainStationLand(byte id) : MonopolyLand(id), IRentLand
{
	[Id(0)] public short Tax { get; set; } = 25;
	[Id(1)] public Guid? OwnerId { get; set; }
	public short Cost  => 200;
	public short SellCost => 100;
	public override void OnLand(MonopolyPlayer player)
	{
		AddPlayer(player);
		if (!OwnerId.HasValue )
		{
			player.AddCommand(new BuyStationCommand());
			player.AddCommand(new CancelBuyCommand());
		}
		if (OwnerId.HasValue && OwnerId != player.Id)
		{
			player.AddCommand(new PayRentCommand(Tax));
		}
	}
	
}
