using TheStarters.Server.Abstractions.Monopoly.Models.Commands;

namespace TheStarters.Server.Abstractions.Monopoly.Models;

[GenerateSerializer, Immutable]
public sealed record MonopolyPlayer(
	Guid Id,
	string Name)
{
	[Id(0)]
	public bool TurnDone { get; set; }
	[Id(1)]
	public IDictionary<byte,MonopolyCommand> Commands { get; } = new Dictionary<byte, MonopolyCommand>();
	[Id(2)] public decimal Balance { get; set; } = 1500;
	[Id(3)] public byte LandId { get; set; }
	public void AddCommand(MonopolyCommand command)
	{
		var id = Commands.Keys.LastOrDefault() + 1;
		command.Id = (byte)id;
		Commands.Add(command.Id, command);
	}
	public void RemoveCommand(byte commandId)
	{
		Commands.Remove(commandId, out var _);
	}
};