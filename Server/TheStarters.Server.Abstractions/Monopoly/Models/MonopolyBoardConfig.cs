using Orleans.Concurrency;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Commands;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;

namespace TheStarters.Server.Abstractions.Monopoly.Models;

public class MonopolyBoardConfig
{
	public List<Type> Lands { get; } = [];

	public IDictionary<byte, Type> LandTypeMap { get; } = new Dictionary<byte, Type>()
	{
		{0, typeof(GoLand)},
		{1, typeof(ChanceLand)},
		{2, typeof(CommunityChestLand)},
		{3, typeof(GoToPrisonLand)},
		{4, typeof(IncomeTaxLand)},
		{5, typeof(ParkingLand)},
		{6, typeof(PrisonLand)},
		{7, typeof(StreetLand)},
		{8, typeof(TrainStationLand)},
	};
}