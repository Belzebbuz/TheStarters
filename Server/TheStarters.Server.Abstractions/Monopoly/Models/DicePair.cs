namespace TheStarters.Server.Abstractions.Monopoly.Models;

[GenerateSerializer, Immutable]
public class DicePair
{
	[Id(0)] public byte First { get; set; }
	[Id(1)] public byte Second { get; set; }
	public byte Total => (byte)(First + Second);

	private readonly Random _random = new();

	public void Next()
	{
		First = Convert.ToByte(_random.Next(1, 6));
		Second = Convert.ToByte(_random.Next(1, 6));
	}
}