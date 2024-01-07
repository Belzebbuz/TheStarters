using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheStarters.Clients.Web.Application.Abstractions.Services.Notifications;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Clients.Web.Application.ClientSubscriptions;

public class TicTacToeObserver(
	IGrainFactory client,
	IServiceProvider provider,
	ILogger<TicTacToeObserver> logger,
	SubRequestChannel requestChannel) : ITicTacToeObserver
{
	public long GameId { get; set; }

	public async ValueTask GameStateChanged()
	{
		var game = await client.GetGrain<ITicTacToeGrain>(GameId).GetAsync();
		var sender = provider.GetRequiredService<INotificationSender>();
		var message = new GameStateChanged(GameType.TicTacToe, GameId);
		var _ = sender.SendToUsersAsync(message,
			[game.Player1.ToString(), game.Player2?.ToString()], new());
		if (game.Winner != null)
			await requestChannel.Requests.Writer.WriteAsync(new GameUnsubscribe(game.GameType, GameId));
		logger.LogInformation($"Состояние игры #{GameId} изменилось.");
	}
}