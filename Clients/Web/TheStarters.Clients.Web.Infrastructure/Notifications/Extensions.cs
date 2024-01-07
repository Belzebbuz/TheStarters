using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace TheStarters.Clients.Web.Infrastructure.Notifications;
internal static class Extensions
{
    internal static IServiceCollection AddNotifications(this IServiceCollection services)
    {
        services.AddSignalR();
        return services;
    }
    internal static IEndpointRouteBuilder MapNotifications(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<NotificationHub>("/api/notifications", options =>
        {
            options.CloseOnAuthenticationExpiration = true;
        });
        return endpoints;
    }
}
