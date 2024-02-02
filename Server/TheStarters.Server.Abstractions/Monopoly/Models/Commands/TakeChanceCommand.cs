using System.Reflection;
using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands;

[GenerateSerializer, Immutable]
public class TakeChanceCommand : MonopolyCommand
{
	public override string Description => CommandDescriptions.TakeChance;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		var chancesTypes = Assembly.GetExecutingAssembly()
			.GetTypes()
			.Where(x => typeof(IChance).IsAssignableFrom(x))
			.ToArray();
		var randomChance = Random.Shared.GetItems(chancesTypes, 1)[0];
		var chance = Activator.CreateInstance(randomChance) as MonopolyCommand ?? throw new GameStateException("Тип не наслдован от команды монополии");
		currentPlayer.AddCommand(chance);
	}
}