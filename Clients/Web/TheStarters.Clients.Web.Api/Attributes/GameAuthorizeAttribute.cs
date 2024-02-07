using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TheStarters.Client.Common.Abstractions;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Clients.Web.Api.Attributes;

public class GameAuthorizeAttribute(GameType gameType) : ActionFilterAttribute
{
	public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		var currentUser = context.HttpContext.RequestServices.GetRequiredService<ICurrentUser>();
		if(!context.ActionArguments.TryGetValue("id", out var idArg))
			return;
		if(idArg is not long id)
			return;
		var grainFactory = context.HttpContext.RequestServices.GetRequiredService<IGrainFactory>();
		var playerGrain = grainFactory.GetGrain<IPlayerGrain>(currentUser.GetUserId());
		var existInGame = await playerGrain.ExistInGame(gameType, id);
		if (existInGame)
			await next();
		else
			context.Result = new BadRequestObjectResult("Пользователь не добавлен в эту сессию");
	}
}