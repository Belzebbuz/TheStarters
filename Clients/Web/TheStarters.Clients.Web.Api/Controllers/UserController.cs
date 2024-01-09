using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orleans.Runtime.Placement;
using TheStarters.Clients.Web.Api.Controllers.Common;
using TheStarters.Clients.Web.Application.Abstractions.Services;
using TheStarters.Clients.Web.Application.Wrapper;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;
using IResult = TheStarters.Clients.Web.Application.Wrapper.IResult;

namespace TheStarters.Clients.Web.Api.Controllers;

[Route("api/[controller]")]
public class UserController : CommonController
{
	[HttpPost("token")]
	[AllowAnonymous]
	public async Task<IResult<TokenResponse>> GetTokenAsync([FromServices] ITokenService tokenService,
		TokenRequest request)
		=> await tokenService.GetTokenAsync(request.Email, request.Password, GetIpAddress(), GetUserAgent());

	[HttpPost("self-register")]
	[AllowAnonymous]
	public async Task<IResult> RegisterAsync([FromServices] IAccountService accountService,
		SelfRegisterRequest request)
	{
		var result = await accountService.SelfRegisterAsync(request);
		var id = new Guid(result.Data.UserId);
		await GrainClient.GetGrain<IPlayerGrain>(id)
			.UpdateProfileAsync(new PlayerProfile(id) { Name = result.Data.Name });
		return Result.Success();
	}

	private string? GetUserAgent() => 
		Request.Headers.TryGetValue("User-Agent", out var value) ? value : "N/A";

	private string? GetIpAddress() =>
		Request.Headers.TryGetValue("X-Forwarded-For", out var value) 
			? value : HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";
}