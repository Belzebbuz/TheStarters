using Microsoft.AspNetCore.Mvc;
using TheStarters.Clients.Web.Api.Controllers.Common;
using TheStarters.Clients.Web.Application.Abstractions.Services;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Clients.Web.Api.Controllers;

[Route("api/[controller]")]
public class GamesController : CommonController
{
	[HttpGet]
	public async Task<List<BaseGame>> GetAllAsync()
		=> await GrainClient.GetGrain<IGamesGrain>(Guid.Empty).GetCreatedAsync();
	[HttpPost]
	public async ValueTask<long> CreateAsync([FromServices] ICurrentUser user, GameType gameType)
		=> await GrainClient.GetGrain<IGameFactoryGrain>(Guid.Empty)
			.CreateGameAsync(gameType, user.GetUserId());
}