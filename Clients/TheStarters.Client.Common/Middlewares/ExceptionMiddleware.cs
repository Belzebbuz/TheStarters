using System.Net;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;
using TheStarters.Client.Common.Abstractions;
using TheStarters.Server.Abstractions.Exceptions;

namespace TheStarters.Client.Common.Middlewares;
internal class ExceptionMiddleware(ICurrentUser currentUser) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var email = currentUser.GetUserEmail() ?? "Anonymous";
            var userId = currentUser.GetUserId();
            if (userId != Guid.Empty) LogContext.PushProperty("UserId", userId);
            LogContext.PushProperty("UserEmail", email);
            var errorId = Guid.NewGuid().ToString();
            LogContext.PushProperty("ErrorId", errorId);
            LogContext.PushProperty("StackTrace", exception.StackTrace);

            var message = exception.Message;
            var response = context.Response;
            response.ContentType = "application/json";

            switch (exception)
            {
                case GameStateException:
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    break;
                case UnauthorizedAccessException: 
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            Log.Error($"{message} Request failed with Status Code {context.Response.StatusCode} and Error Id {errorId}.\nStackTrace:{exception.StackTrace}");
            await response.WriteAsync(message);
        }
    }
}
