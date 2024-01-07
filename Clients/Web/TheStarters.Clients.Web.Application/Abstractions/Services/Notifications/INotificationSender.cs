﻿using TheStarters.Clients.Web.Application.Abstractions.DI;

namespace TheStarters.Clients.Web.Application.Abstractions.Services.Notifications;

public interface INotificationSender : ITransientService
{
	ValueTask SendToAllAsync(INotificationMessage message, CancellationToken cancellationToken);
	
	ValueTask SendToUserAsync(INotificationMessage notification, string userId,
		CancellationToken cancellationToken);
	
	ValueTask SendToUsersAsync(INotificationMessage notification, List<string?> userIds,
		CancellationToken cancellationToken);
}