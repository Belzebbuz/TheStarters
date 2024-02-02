using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TheStarters.Client.Common.Abstractions.DI;

namespace TheStarters.Clients.Web.Infrastructure.Notifications;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class NotificationHub(ILogger<NotificationHub> logger) : Hub, ITransientService
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        logger.LogInformation($"A client connected to notification hub: {Context.ConnectionId} {Context.UserIdentifier}");
    }
}
