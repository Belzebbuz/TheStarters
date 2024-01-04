using Microsoft.AspNetCore.Http;
using TheStarters.Clients.Web.Application.Abstractions.Services;

namespace TheStarters.Clients.Web.Infrastructure.Middlewares.RequestCurrentUser;
internal class CurrentUserMiddleware : IMiddleware
{
    private readonly ICurrentUserInitializer _currentUserInitializer;

    public CurrentUserMiddleware(ICurrentUserInitializer currentUserInitializer) =>
        _currentUserInitializer = currentUserInitializer;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _currentUserInitializer.SetCurrentUser(context.User);

        await next(context);
    }
}
