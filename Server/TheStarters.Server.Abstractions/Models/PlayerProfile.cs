namespace TheStarters.Server.Abstractions.Models;

[GenerateSerializer, Immutable]
public record PlayerProfile(Guid Id)
{
	[Id(1)]
	public string? Name { get; set; }
}