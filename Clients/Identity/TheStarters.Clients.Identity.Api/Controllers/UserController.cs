using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheStarters.Clients.Identity.Api.Abstractions;

namespace TheStarters.Clients.Identity.Api.Controllers;

[Route("api/identity/[controller]")]
public class UserController : CommonController
{
	[HttpPost("token")]
	[AllowAnonymous]
	public async Task<ActionResult<TokenResponse>> GetTokenAsync(
		[FromServices] ITokenService tokenService,
		TokenRequest request)
	{
		var result = await tokenService.GetTokenAsync(request.Email, request.Password, GetIpAddress(), GetUserAgent());
		return result.Match(value => Ok(value), Problem);
			
	}
	
	[HttpPost("refresh-token")]
	[AllowAnonymous]
	public async Task<ActionResult<TokenResponse>> RefreshTokenAsync(
		[FromServices] ITokenService tokenService,
		RefreshTokenRequest request)
	{
		var result = await tokenService.RefreshTokenAsync(request, GetIpAddress(), GetUserAgent());
		return result.Match(value => Ok(value), Problem);
	}
	
	[HttpPost("self-register")]
	[AllowAnonymous]
	public async Task<IActionResult> RegisterAsync([FromServices] IAccountService accountService,
		SelfRegisterRequest request)
	{
		var result = await accountService.SelfRegisterAsync(request);
		return result.Match(value => Ok(), Problem);
	}

	private string GetUserAgent() => 
		Request.Headers.TryGetValue("User-Agent", out var value) ? value.ToString() : "N/A";

	private string? GetIpAddress() =>
		Request.Headers.TryGetValue("X-Forwarded-For", out var value) 
			? value : HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";
}