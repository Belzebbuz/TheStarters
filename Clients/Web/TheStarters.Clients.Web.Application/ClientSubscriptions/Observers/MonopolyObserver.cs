using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheStarters.Clients.Web.Application.Abstractions.Services.Notifications;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Abstractions.Monopoly;

namespace TheStarters.Clients.Web.Application.ClientSubscriptions.Observers;

public class MonopolyObserver(
	IGrainFactory client,
	IServiceProvider provider,
	ILogger<MonopolyObserver> logger,
	SubRequestChannel requestChannel) : IMonopolyObserver
{
	public long GameId { get; set; }
	public async Task GameStateChanged()
	{
		var game = await client.GetGrain<IMonopolyGrain>(GameId).GetAsync();
		var users = game.Players.Keys.Select(x => x.ToString()).ToList();
		await using var scope = provider.CreateAsyncScope();
		var sender = scope.ServiceProvider.GetRequiredService<INotificationSender>();
		var message = new GameStateChanged(GameType.Monopoly, GameId);
		var _ = sender.SendToUsersAsync(
			message, users, new CancellationToken());
		if(game.Winner is not null)
			await requestChannel.Requests.Writer.WriteAsync(new GameUnsubscribe(game.GameType, GameId));
		logger.LogInformation($"Состояние игры #{GameId} изменилось.");
	}

	public Task BuyRequestsChanged(Guid ownerId)
	{
		var message = new BuyLandRequestChanged(GameType.Monopoly, GameId);
		var sender = provider.GetRequiredService<INotificationSender>();
		var _ = sender.SendToUserAsync(message, ownerId.ToString(), new());
		logger.LogInformation($"Игроку Id:{ownerId} отправлен запрос на покупку поля.");
		return Task.CompletedTask;
	}
}