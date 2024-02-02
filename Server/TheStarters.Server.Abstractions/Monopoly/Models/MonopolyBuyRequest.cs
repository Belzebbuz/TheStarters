namespace TheStarters.Server.Abstractions.Monopoly.Models;

[GenerateSerializer, Immutable]
public class MonopolyBuyRequest
{
	[Id(0)] public Guid Id { get; init; }
	[Id(1)] public Guid AuthorId { get; init; }
	[Id(2)] public Guid OwnerId { get; init; }
	[Id(3)] public byte LandId { get; init; }
	[Id(4)] public DateTime CreatedOn { get; init; }
}