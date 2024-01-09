using System.Text.Json.Serialization;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands;

[JsonDerivedType(typeof(MoveCommand), nameof(MoveCommand))]
[JsonDerivedType(typeof(BuyCompanyCommand), nameof(BuyCompanyCommand))]
[JsonDerivedType(typeof(PayRentCommand), nameof(PayRentCommand))]
[GenerateSerializer, Immutable]
public abstract class MonopolyCommand(bool required)
{
	[Id(0)]
	public byte Id { get; set; }
	
	[Id(1)]
	public string Description { get; set; } = string.Empty;
    
	[Id(2)]
	public bool Required { get; } = required;

	public abstract void Execute(MonopolyGame game);
}