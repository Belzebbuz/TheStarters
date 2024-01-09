using Microsoft.AspNetCore.Mvc;
using TheStarters.Clients.Web.Api.Controllers.Common;
using TheStarters.Clients.Web.Application.Abstractions.Services;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Abstractions.Monopoly;
using TheStarters.Server.Abstractions.Monopoly.Models;
using TheStarters.Server.Abstractions.Monopoly.Models.Commands;

namespace TheStarters.Clients.Web.Api.Controllers;

[Route("api/[controller]")]
public class MonopolyController(ICurrentUser user) : CommonController
{
	[HttpGet("{gameId}")]
	public async ValueTask<MonopolyGame> GetAsync(long gameId)
		=> await GrainClient.GetGrain<IMonopolyGrain>(gameId).GetAsync();

	[HttpPut("{id}/start")]
	public async ValueTask StartAsync(long id)
		=> await GrainClient.GetGrain<IMonopolyGrain>(id).StartGameAsync();
	
	[HttpPut("{id}/execute-command")]
	public async ValueTask ExecuteCommandAsync(long id, MonopolyCommand command)
		=> await GrainClient.GetGrain<IMonopolyGrain>(id).ExecuteCommandAsync(user.GetUserId(), command);
	
	[HttpPut("{id}/end-turn")]
	public async ValueTask EndTurnAsync(long id)
		=> await GrainClient.GetGrain<IMonopolyGrain>(id).EndTurnAsync(user.GetUserId());
}