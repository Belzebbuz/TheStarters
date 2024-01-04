using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TheStarters.Clients.Web.Api.Controllers.Common;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CommonController : ControllerBase
{
	private IGrainFactory _client;
	protected IGrainFactory GrainClient => _client ??= HttpContext.RequestServices.GetRequiredService<IGrainFactory>();
}