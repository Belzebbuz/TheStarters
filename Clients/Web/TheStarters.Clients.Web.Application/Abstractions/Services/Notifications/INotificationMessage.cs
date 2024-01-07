using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Clients.Web.Application.Abstractions.Services.Notifications;

public interface INotificationMessage
{
}

public record GameStateChanged(GameType GameType, long GameId) : INotificationMessage;