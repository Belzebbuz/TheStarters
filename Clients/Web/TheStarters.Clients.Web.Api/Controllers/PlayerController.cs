using Microsoft.AspNetCore.Mvc;
using TheStarters.Client.Common.Abstractions;
using TheStarters.Clients.Web.Api.Controllers.Common;
using TheStarters.Clients.Web.Application.Abstractions.Services;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Clients.Web.Api.Controllers;

[Route("api/[controller]")]
public class PlayerController(ICurrentUser user) : CommonController
{
	[HttpGet("profile")]
	public async Task<PlayerProfile> GetProfileAsync()
		=> await GrainClient.GetGrain<IPlayerGrain>(user.GetUserId())
			.GetProfileAsync();
	
	[HttpPut("profile")]
	public async Task GetProfileAsync(
		string name)
		=> await GrainClient.GetGrain<IPlayerGrain>(user.GetUserId())
			.SetNameAsync(name);
}