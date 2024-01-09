using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheStarters.Clients.Web.Application.Abstractions.DI;
using TheStarters.Clients.Web.Application.Abstractions.Services.Notifications;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Abstractions.Monopoly;

namespace TheStarters.Clients.Web.Application.ClientSubscriptions;

public class MonopolyObserver(
	IGrainFactory client,
	IServiceProvider provider,
	ILogger<TicTacToeObserver> logger,
	SubRequestChannel requestChannel) : IMonopolyObserver
{
	public long GameId { get; set; }
	public async ValueTask GameStateChanged()
	{
		var game = await client.GetGrain<IMonopolyGrain>(GameId).GetAsync();
		var users = game.Players.Select(x => x.Id.ToString()).ToList();
		var sender = provider.GetRequiredService<INotificationSender>();
		var message = new GameStateChanged(GameType.Monopoly, GameId);
		var _ = sender.SendToUsersAsync(
			message, users, new CancellationToken());
		if(game.Winner is not null)
			await requestChannel.Requests.Writer.WriteAsync(new GameUnsubscribe(game.GameType, GameId));
		logger.LogInformation($"Состояние игры #{GameId} изменилось.");
	}
}