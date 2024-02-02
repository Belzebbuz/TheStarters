using Microsoft.AspNetCore.Mvc;
using TheStarters.Client.Common.Abstractions;
using TheStarters.Clients.Web.Api.Controllers.Common;
using TheStarters.Clients.Web.Application.Abstractions.Services;
using TheStarters.Clients.Web.Application.ClientSubscriptions;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Clients.Web.Api.Controllers;

[Route("api/[controller]")]
public class GamesController: CommonController
{
	[HttpGet]
	public async Task<IEnumerable<BaseGame>> GetAllAsync(int page, int pageSize)
		=> (await GrainClient.GetGrain<IGamesListGrain>(Guid.Empty).GetAsync(page,pageSize)).Value;
	
	[HttpPost]
	public async ValueTask<long> CreateAsync([FromServices] ICurrentUser user, GameType gameType)
	{
		var gameId =  await GrainClient.GetGrain<IGameFactoryGrain>(Guid.Empty)
			.CreateGameAsync(gameType, user.GetUserId());
		return gameId;
	}
}