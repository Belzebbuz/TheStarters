using Microsoft.AspNetCore.Mvc;
using TheStarters.Client.Common.Abstractions;
using TheStarters.Clients.Web.Api.Controllers.Common;
using TheStarters.Clients.Web.Application.Abstractions.Services;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Clients.Web.Api.Controllers;

[Route("api/[controller]")]
public class TicTacToeController(ICurrentUser user) : CommonController
{
	[HttpPut("{gameId}/join-game")]
	public async ValueTask AddPlayerAsync(long gameId)
		=> await GrainClient.GetGrain<ITicTacToeGrain>(gameId).AddPlayerAsync(user.GetUserId());
	
	[HttpPut("{gameId}/remove-player")]
	public async ValueTask RemovePlayerAsync(long gameId)
		=> await GrainClient.GetGrain<ITicTacToeGrain>(gameId).RemovePlayerAsync(user.GetUserId());
	
	[HttpPut("{gameId}/start")]
	public async ValueTask StartAsync(long gameId)
		=> await GrainClient.GetGrain<ITicTacToeGrain>(gameId).StartAsync(user.GetUserId());

	[HttpPut("{gameId}/answer")]
	public async ValueTask SetAnswerAsync(long gameId, byte x, byte y)
		=> await GrainClient.GetGrain<ITicTacToeGrain>(gameId).SetAnswerAsync(user.GetUserId(), x, y);

	[HttpGet("{gameId}")]
	public async ValueTask<TicTacToeGame> GetAsync(long gameId) 
		=> await GrainClient.GetGrain<ITicTacToeGrain>(gameId).GetAsync();
}