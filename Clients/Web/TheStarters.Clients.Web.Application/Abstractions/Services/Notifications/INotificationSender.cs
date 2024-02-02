using TheStarters.Client.Common.Abstractions.DI;

namespace TheStarters.Clients.Web.Application.Abstractions.Services.Notifications;

public interface INotificationSender : ITransientService
{
	ValueTask SendToAllAsync(INotificationMessage message, CancellationToken cancellationToken);
	
	ValueTask SendToUserAsync(INotificationMessage notification, string userId,
		CancellationToken cancellationToken);
	
	ValueTask SendToUsersAsync(INotificationMessage notification, IEnumerable<string?> userIds,
		CancellationToken cancellationToken);
}