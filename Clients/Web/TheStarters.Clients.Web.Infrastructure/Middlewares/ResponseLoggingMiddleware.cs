using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;
using TheStarters.Clients.Web.Application.Abstractions.Services;

namespace TheStarters.Clients.Web.Infrastructure.Middlewares;
internal class ResponseLoggingMiddleware : IMiddleware
{
    private readonly ICurrentUser _currentUser;

    public ResponseLoggingMiddleware(ICurrentUser currentUser) => _currentUser = currentUser;

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        await next(httpContext);
        var originalBody = httpContext.Response.Body;
        using var newBody = new MemoryStream();
        httpContext.Response.Body = newBody;
        string responseBody;
        if (httpContext.Request.Path.ToString().Contains("tokens"))
            responseBody = "[Redacted] Contains Sensitive Information.";
        else
        {
            newBody.Seek(0, SeekOrigin.Begin);
            responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        }

        string email = _currentUser.GetUserEmail() is string userEmail ? userEmail : "Anonymous";
        var userId = _currentUser.GetUserId();
        if (userId != Guid.Empty) LogContext.PushProperty("UserId", userId);
        LogContext.PushProperty("UserEmail", email);
        LogContext.PushProperty("StatusCode", httpContext.Response.StatusCode);
        LogContext.PushProperty("ResponseTimeUTC", DateTime.UtcNow);
        Log.ForContext("ResponseHeaders", httpContext.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
            .ForContext("ResponseBody", responseBody)
            .Information($"HTTP {httpContext.Request.Method} Request to {httpContext.Request.Path} by {email} has Status Code {httpContext.Response.StatusCode}.");
        newBody.Seek(0, SeekOrigin.Begin);
        await newBody.CopyToAsync(originalBody);
    }
}
