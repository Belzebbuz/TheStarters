namespace TheStarters.Server.Abstractions.Models;

[GenerateSerializer, Immutable]
public record PlayerProfile
{
	[Id(0)]
	public string? Name { get; set; }
}