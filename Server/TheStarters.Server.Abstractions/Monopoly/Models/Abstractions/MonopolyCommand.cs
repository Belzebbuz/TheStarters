using System.Text.Json.Serialization;
using TheStarters.Server.Abstractions.Monopoly.Models.Commands;
using TheStarters.Server.Abstractions.Monopoly.Models.Commands.Chances;
using TheStarters.Server.Abstractions.Monopoly.Models.Commands.CommunityChests;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;

[JsonDerivedType(typeof(MoveCommand), nameof(MoveCommand))]
[JsonDerivedType(typeof(BuyStreetCommand), nameof(BuyStreetCommand))]
[JsonDerivedType(typeof(BuyStationCommand), nameof(BuyStationCommand))]
[JsonDerivedType(typeof(CancelBuyCommand), nameof(CancelBuyCommand))]
[JsonDerivedType(typeof(GoToPrisonCommand), nameof(GoToPrisonCommand))]
[JsonDerivedType(typeof(PayForFreeCommand), nameof(PayForFreeCommand))]
[JsonDerivedType(typeof(PayRentCommand), nameof(PayRentCommand))]
[JsonDerivedType(typeof(PayIncomeTaxCommand), nameof(PayIncomeTaxCommand))]
[JsonDerivedType(typeof(ThrowDiceCommand), nameof(ThrowDiceCommand))]
[JsonDerivedType(typeof(TakeChanceCommand), nameof(TakeChanceCommand))]
[JsonDerivedType(typeof(TakeCommunityChestCommand), nameof(TakeCommunityChestCommand))]
// [JsonDerivedType(typeof(ChairmanBoardOfDirectorsChance), nameof(ChairmanBoardOfDirectorsChance))]
// [JsonDerivedType(typeof(GoToGoLandChance), nameof(GoToGoLandChance))]
// [JsonDerivedType(typeof(GoToPrisonChance), nameof(GoToPrisonChance))]
// [JsonDerivedType(typeof(GoToRandomStreetChance), nameof(GoToRandomStreetChance))]
// [JsonDerivedType(typeof(GoToStationChance), nameof(GoToStationChance))]
// [JsonDerivedType(typeof(MajorRenovationChance), nameof(MajorRenovationChance))]
// [JsonDerivedType(typeof(SpeedingFineChance), nameof(SpeedingFineChance))]
// [JsonDerivedType(typeof(TakeDividendsChance), nameof(TakeDividendsChance))]
// [JsonDerivedType(typeof(BankErrorChest), nameof(BankErrorChest))]
// [JsonDerivedType(typeof(BirthdayGiftChest), nameof(BirthdayGiftChest))]
// [JsonDerivedType(typeof(DoctorVisitChest), nameof(DoctorVisitChest))]
[GenerateSerializer, Immutable]
public abstract class MonopolyCommand
{
	[Id(0)] public int Id { get; set; }
	public abstract string Description { get; }
	public abstract void Execute(MonopolyGame game, MonopolyPlayer currentPlayer);
}