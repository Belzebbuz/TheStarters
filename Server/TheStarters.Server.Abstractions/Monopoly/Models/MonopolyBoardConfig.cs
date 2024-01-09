using Orleans.Concurrency;

namespace TheStarters.Server.Abstractions.Monopoly.Models;

[GenerateSerializer, Immutable]
public class MonopolyBoardConfig
{
	public List<Type> Lands { get; } = [];
}