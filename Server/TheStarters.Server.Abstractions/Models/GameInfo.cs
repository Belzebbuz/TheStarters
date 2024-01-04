namespace TheStarters.Server.Abstractions.Models;

[GenerateSerializer, Immutable]
public record GameInfo
{
	[Id(0)]
	public required string Name { get; set; }
}