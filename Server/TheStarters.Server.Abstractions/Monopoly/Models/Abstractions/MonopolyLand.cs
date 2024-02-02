using System.Text.Json.Serialization;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;

[JsonDerivedType(typeof(GoLand),nameof(GoLand))]
[JsonDerivedType(typeof(GoToPrisonLand),nameof(GoToPrisonLand))]
[JsonDerivedType(typeof(StreetLand), nameof(StreetLand))]
[JsonDerivedType(typeof(IncomeTaxLand), nameof(IncomeTaxLand))]
[JsonDerivedType(typeof(ParkingLand), nameof(ParkingLand))]
[JsonDerivedType(typeof(TrainStationLand), nameof(TrainStationLand))]
[JsonDerivedType(typeof(PrisonLand), nameof(PrisonLand))]
[JsonDerivedType(typeof(ChanceLand), nameof(ChanceLand))]
[JsonDerivedType(typeof(CommunityChestLand), nameof(CommunityChestLand))]
[GenerateSerializer, Immutable]
public abstract class MonopolyLand(byte id)
{
	[Id(0)] public byte Id { get; } = id;
	public abstract void OnLand(MonopolyPlayer player);
	
	protected void AddPlayer(MonopolyPlayer player)
	{
		player.LandId = Id;
	}
}