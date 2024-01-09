using System.Text.Json.Serialization;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Lands;

[JsonDerivedType(typeof(StartLand),nameof(StartLand))]
[JsonDerivedType(typeof(CompanyLand), nameof(CompanyLand))]
[GenerateSerializer, Immutable]
public abstract class MonopolyLand(byte id)
{
	[Id(0)] public byte Id { get; } = id;
	[Id(1)] public IDictionary<Guid,MonopolyPlayer> CurrentPlayers { get; } = new Dictionary<Guid, MonopolyPlayer>();
	public abstract void OnLand(MonopolyPlayer player);

	protected virtual void AddPlayer(MonopolyPlayer player)
	{
		CurrentPlayers.Add(player.Id, player);
		player.LandId = Id;
	}
}