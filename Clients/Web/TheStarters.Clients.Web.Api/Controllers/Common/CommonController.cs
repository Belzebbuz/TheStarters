using ErrorOr;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TheStarters.Clients.Web.Api.Controllers.Common;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CommonController : ControllerBase
{
	private IGrainFactory _client;
	protected IGrainFactory GrainClient => _client ??= HttpContext.RequestServices.GetRequiredService<IGrainFactory>();

	protected ActionResult Problem(List<Error> errors)
	{
		if (!errors.Any())
		{
			return Problem();
		}
		if (errors.All(error => error.Type == ErrorType.Validation))
		{
			return ValidationProblem(errors);
		}

		return Problem(errors[0]);
	}
	private ObjectResult Problem(Error error)
	{
		var statusCode = error.Type switch
		{
			ErrorType.Conflict => StatusCodes.Status409Conflict,
			ErrorType.Validation => StatusCodes.Status400BadRequest,
			ErrorType.NotFound => StatusCodes.Status404NotFound,
			ErrorType.Unauthorized => StatusCodes.Status403Forbidden,
			_ => StatusCodes.Status500InternalServerError,
		};

		return Problem(statusCode: statusCode, title: error.Description);
	}
	private ActionResult ValidationProblem(List<Error> errors)
	{
		var modelStateDictionary = new ModelStateDictionary();

		errors.ForEach(error => modelStateDictionary.AddModelError(error.Code, error.Description));

		return ValidationProblem(modelStateDictionary);
	}
}