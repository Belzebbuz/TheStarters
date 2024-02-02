using TheStarters.Server.Abstractions.Exceptions;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Consts;

namespace TheStarters.Server.Abstractions.Monopoly.Models.Commands;

[GenerateSerializer, Immutable]
public class PayIncomeTaxCommand(short defaultValue) : MonopolyCommand
{
	public override string Description => CommandDescriptions.PayIncomeTax;
	[Id(0)]public short DefaultValue { get; } = defaultValue;
	public override void Execute(MonopolyGame game, MonopolyPlayer currentPlayer)
	{
		if (currentPlayer.Balance < DefaultValue)
			throw new GameStateException("У игрока недостаточно средств для оплаты налога.");
		currentPlayer.Balance -= DefaultValue;
		currentPlayer.RemoveCommand(Id);
	}
}