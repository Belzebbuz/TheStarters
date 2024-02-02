using Microsoft.AspNetCore.Mvc;
using TheStarters.Client.Common.Abstractions;
using TheStarters.Clients.Web.Api.Controllers.Common;
using TheStarters.Clients.Web.Application.Abstractions.Services;
using TheStarters.Server.Abstractions.Monopoly;
using TheStarters.Server.Abstractions.Monopoly.Models;
using TheStarters.Server.Abstractions.Monopoly.Models.Commands;

namespace TheStarters.Clients.Web.Api.Controllers;

[Route("api/[controller]/{id:long}")]
public class MonopolyController(ICurrentUser user) : CommonController
{
	[HttpGet]
	public async ValueTask<MonopolyGame> GetAsync(long id)
		=> await GrainClient.GetGrain<IMonopolyGrain>(id).GetAsync();
	
	[HttpPost("join-game")]
	public async ValueTask AddPlayerAsync(long id)
		=> await GrainClient.GetGrain<IMonopolyGrain>(id)
			.AddPlayerAsync(user.GetUserId());
	
	[HttpPost("leave-game")]
	public async ValueTask RemovePlayerAsync(long id)
		=> await GrainClient.GetGrain<IMonopolyGrain>(id)
			.RemovePlayerAsync(user.GetUserId());
	
	[HttpPost("start")]
	public async ValueTask StartAsync(long id)
		=> await GrainClient.GetGrain<IMonopolyGrain>(id).StartGameAsync(user.GetUserId());
	
	[HttpPost("execute-command")]
	public async ValueTask ExecuteCommandAsync(long id, int commandId)
		=> await GrainClient.GetGrain<IMonopolyGrain>(id)
			.ExecuteCommandAsync(user.GetUserId(), commandId);
	
	[HttpPost("end-turn")]
	public async ValueTask EndTurnAsync(long id)
		=> await GrainClient.GetGrain<IMonopolyGrain>(id)
			.EndTurnAsync(user.GetUserId());
	
	[HttpPost("execute-land-operation")]
	public async ValueTask ExecuteLandOperationAsync(long id, byte landId, LandOperation operation)
		=> await GrainClient.GetGrain<IMonopolyGrain>(id)
			.ExecuteLandOperationAsync(user.GetUserId(), landId, operation);
	
	[HttpGet("buy-requests")]
	public async ValueTask GetBuyRequestsAsync(long id)
		=> await GrainClient.GetGrain<IMonopolyGrain>(id)
			.GetOwnerBuyRequestsAsync(user.GetUserId());
	
	[HttpPost("buy-requests")]
	public async ValueTask CreateBuyRequestsAsync(long id,byte landId)
		=> await GrainClient.GetGrain<IMonopolyGrain>(id)
			.CreateBuyLandRequestAsync(user.GetUserId(),landId);
	
	[HttpPut("buy-requests")]
	public async ValueTask ConfirmBuyRequestsAsync(long id, Guid buyRequestId)
		=> await GrainClient.GetGrain<IMonopolyGrain>(id)
			.ConfirmBuyRequestAsync(user.GetUserId(),buyRequestId);
}