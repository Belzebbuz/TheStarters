using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheStarters.Clients.Web.Application.Abstractions.Services.Notifications;
using TheStarters.Server.Abstractions;

namespace TheStarters.Clients.Web.Application.ClientSubscriptions.Observers;

public class GameListObserver(
	IServiceProvider provider,
	ILogger<GameListObserver> logger) : IGamesListObserver
{
	public async Task ListStateChanged()
	{
		await using var scope = provider.CreateAsyncScope();
		var sender = scope.ServiceProvider.GetRequiredService<INotificationSender>();
		var _ = sender.SendToAllAsync(new GameListChanged(), new());
		logger.LogInformation("Список игр обновился");
	}
}