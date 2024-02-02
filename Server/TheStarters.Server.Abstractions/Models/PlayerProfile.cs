namespace TheStarters.Server.Abstractions.Models;

[GenerateSerializer, Immutable]
public record PlayerProfile
{
	[Id(0)] public Guid Id { get; set; }
	[Id(1)] public string Name { get; set; } = string.Empty;
}