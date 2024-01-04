using System.Net;

namespace TheStarters.Clients.Web.Application.Abstractions.Exceptions;

public abstract class BaseApplicationException : Exception
{
	public HttpStatusCode StatusCode { get; init; }
	public List<string>? ErrorMessages { get; }
	
	public BaseApplicationException(string message, List<string>? errors = default, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
		: base(message)
	{
		ErrorMessages = errors;
		StatusCode = statusCode;
	}
}