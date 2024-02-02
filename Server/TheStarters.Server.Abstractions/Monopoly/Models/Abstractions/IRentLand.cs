namespace TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;

public interface IRentLand
{
	public Guid? OwnerId { get; set; }
	public short Tax { get; }
	public short SellCost { get;}
}