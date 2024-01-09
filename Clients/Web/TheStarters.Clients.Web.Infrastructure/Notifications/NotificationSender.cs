using Microsoft.AspNetCore.SignalR;
using TheStarters.Clients.Web.Application.Abstractions.Services.Notifications;

namespace TheStarters.Clients.Web.Infrastructure.Notifications;

public class NotificationSender(
	IHubContext<NotificationHub> hubContext)
	: INotificationSender
{
	private readonly string _channel = "NotificationServer";

	public async ValueTask SendToAllAsync(INotificationMessage message, CancellationToken cancellationToken)
		=> await hubContext.Clients.All.SendAsync(_channel, message.GetType().Name, message, cancellationToken);

	public async ValueTask SendToUserAsync(INotificationMessage notification, string userId,
		CancellationToken cancellationToken)
		=> await hubContext.Clients.User(userId)
			.SendAsync(_channel, notification.GetType().Name, notification, cancellationToken);

	public async ValueTask SendToUsersAsync(INotificationMessage notification, IEnumerable<string?> userIds, CancellationToken cancellationToken)
	{
		foreach (var userId in userIds.OfType<string>())
		{
			await SendToUserAsync(notification, userId, cancellationToken);
		}
	}
}