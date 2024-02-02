using Microsoft.Extensions.Logging;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Clients.Web.Application.ClientSubscriptions.Observers;

public class GameFactoryObserver(
	ILogger<GameFactoryObserver> logger,
	SubRequestChannel requestChannel) : IGameFactoryObserver
{
	public Task GameCreated(GameType gameType, long id)
	{
		requestChannel.Requests.Writer.WriteAsync(new GameSubscribe(gameType, id));
		logger.LogInformation($"Создана игра {gameType}. Id: {id}");
		return Task.CompletedTask;
	}
}