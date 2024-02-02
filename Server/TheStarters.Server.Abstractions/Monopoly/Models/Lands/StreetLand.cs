using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Commands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Lands;

[GenerateSerializer, Immutable]
public sealed class StreetLand : MonopolyLand, IRentLand
{
	public StreetLand(byte id, string name, short cost, short houseCost, string city)
		: base(id)
	{
		Name = name;
		Cost = cost;
		City = city;
		HouseCost = houseCost;
	}

	[Id(0)] public string Name { get; }
	[Id(1)] public short Cost { get; }
	[Id(2)] public string City { get; }
	[Id(3)] public Guid? OwnerId { get; set; }
	[Id(4)] public byte HouseCount { get; set; }
	[Id(5)] public short HouseCost { get; }
	[Id(6)] public bool HasHotel { get; set; }
	public short Tax => CalculateTax(HouseCount, HasHotel);
	public short HotelCost => (short)(HouseCost * 5);
	public ICollection<string> TaxInfo => GetTaxInfo();
	public short SellCost => (short)(Cost / 2);

	public override void OnLand(MonopolyPlayer player)
	{
		AddPlayer(player);
		if (!OwnerId.HasValue)
		{
			player.AddCommand(new BuyStreetCommand());
			player.AddCommand(new CancelBuyCommand());
		}

		if (OwnerId.HasValue && OwnerId != player.Id)
		{
			player.AddCommand(new PayRentCommand(Tax));
		}
	}

	private short CalculateTax(byte houseCount, bool hotelExist)
	{
		if (hotelExist)
			return (short)(Cost * 5);
		return houseCount switch
		{
			0 => (short)Math.Round(Cost * 0.06),
			1 => (short)Math.Round(Cost * 0.16),
			2 => (short)Math.Round(Cost * 0.5),
			3 => (short)Math.Round(Cost * 1.5),
			4 => (short)Math.Round(Cost * 2.6),
			_ => 0
		};
	}

	private List<string> GetTaxInfo() =>
	[
		$"Арендная плата за участок: {CalculateTax(0, false)}",
		$"C одним домом: {CalculateTax(1, false)}",
		$"C двумя домами: {CalculateTax(2, false)}",
		$"C тремя домами: {CalculateTax(3, false)}",
		$"C четырьмя домами: {CalculateTax(4, false)}",
		$"C одним отелем: {CalculateTax(0, true)}"
	];
}