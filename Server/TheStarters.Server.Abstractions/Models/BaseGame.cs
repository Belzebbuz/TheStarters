namespace TheStarters.Server.Abstractions.Models;

[GenerateSerializer, Immutable]
public record BaseGame
{
	[Id(0)] public long Id { get; set; }
	[Id(1)] public byte PlayersCount { get; set; } = 1;
	[Id(2)] public bool Started { get; set; }
	[Id(3)] public Guid? Winner { get; set; }
	[Id(4)] public DateTime CreatedOn { get; set; }
	[Id(5)] public DateTime LastUpdateOn { get; set; }
	[Id(6)] public GameType GameType { get; init; }

	public void Update() => LastUpdateOn = DateTime.UtcNow;
}