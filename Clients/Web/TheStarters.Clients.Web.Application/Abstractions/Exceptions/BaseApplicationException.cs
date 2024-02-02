using System.Net;

namespace TheStarters.Clients.Web.Application.Abstractions.Exceptions;

public abstract class BaseApplicationException : Exception
{
	public HttpStatusCode StatusCode { get; init; }
	public string? ErrorMessage { get; }
	
	public BaseApplicationException(string message, string? error = default, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
		: base(message)
	{
		ErrorMessage = error;
		StatusCode = statusCode;
	}
}