using System.Net;
using Microsoft.AspNetCore.Http;
using OneOf.Types;
using Serilog;
using Serilog.Context;
using TheStarters.Clients.Web.Application.Abstractions.Exceptions;
using TheStarters.Clients.Web.Application.Abstractions.Services;
using TheStarters.Clients.Web.Application.Wrapper;

namespace TheStarters.Clients.Web.Infrastructure.Middlewares;
internal class ExceptionMiddleware : IMiddleware
{
    private readonly ICurrentUser _currentUser;
    private readonly ISerializerService _jsonSerializer;
    public ExceptionMiddleware(
        ICurrentUser currentUser,
        ISerializerService jsonSerializer)
    {
        _currentUser = currentUser;
        _jsonSerializer = jsonSerializer;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var email = _currentUser.GetUserEmail() ?? "Anonymous";
            var userId = _currentUser.GetUserId();
            if (userId != Guid.Empty) LogContext.PushProperty("UserId", userId);
            LogContext.PushProperty("UserEmail", email);
            var errorId = Guid.NewGuid().ToString();
            LogContext.PushProperty("ErrorId", errorId);
            LogContext.PushProperty("StackTrace", exception.StackTrace);

            var errorResult = await Result.FailAsync(exception.Message);
            var response = context.Response;
            response.ContentType = "application/json";

            switch (exception)
            {
                case BaseApplicationException e:
                    response.StatusCode = (int)e.StatusCode;
                    if (e.ErrorMessages is not null)
                        errorResult.Messages = e.ErrorMessages;

                    break;

                case UnauthorizedAccessException: 
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            Log.Error($"{errorResult.Messages.Last()} Request failed with Status Code {context.Response.StatusCode} and Error Id {errorId}.");
            await response.WriteAsync(_jsonSerializer.Serialize(errorResult));
        }
    }
}
