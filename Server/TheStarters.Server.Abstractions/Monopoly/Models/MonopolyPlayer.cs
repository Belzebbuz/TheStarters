using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;

namespace TheStarters.Server.Abstractions.Monopoly.Models;

[GenerateSerializer, Immutable]
public sealed record MonopolyPlayer(
	Guid Id,
	string Name)
{
	[Id(0)] public bool TurnDone { get; set; }
	[Id(1)] public IDictionary<int, MonopolyCommand> Commands { get; } = new Dictionary<int, MonopolyCommand>();
	[Id(2)] public ICollection<byte> OwnedProperties { get; } = new HashSet<byte>();
	[Id(3)] public decimal Balance { get; set; } = 1500;
	[Id(4)] public byte LandId { get; set; }
	[Id(5)] public bool Arrested { get; set; }
	[Id(6)] public byte TurnDiceDoubles { get; set; }
	public void AddCommand(MonopolyCommand command)
	{
		var id = Commands.Keys.LastOrDefault() + 1;
		command.Id = id;
		Commands.Add(command.Id, command);
	}

	public void RemoveCommand(int commandId)
	{
		Commands.Remove(commandId, out var _);
	}
};