using System.Collections.Concurrent;
using Mapster;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Core;
using Orleans.Runtime;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Grains.Consts;

namespace TheStarters.Server.Grains;

public class GamesListGrain(
	[PersistentState(stateName: "created-games", storageName: StorageConsts.PersistenceStorage)]
	IStorage<Dictionary<long, BaseGame>> createdGames,
	ILogger<GamesListGrain> logger)
	: ObservableGrain<IGamesListObserver>(logger), IGamesListGrain
{
	private readonly ILogger<GamesListGrain> _logger = logger;

	public ValueTask<Immutable<ICollection<BaseGame>>> GetAsync(int page, int pageSize)
	{
		var result = createdGames.State.Values
			.OrderByDescending(x => x.CreatedOn)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToList();
		return ValueTask.FromResult(
			new Immutable<ICollection<BaseGame>>(result));
	}

	public async ValueTask AddOrUpdateGameAsync(BaseGame game)
	{
		var baseGame = game with { };
		createdGames.State[game.Id] = baseGame;
		await createdGames.WriteStateAsync();
		await NotifyAsync();
	}

	public ValueTask<Immutable<ICollection<BaseGame>>> GetLastUpdatedAsync()
	{
		var result = createdGames.State.Values
			.Where(x => !x.Winner.HasValue)
			.Where(x => x.LastUpdateOn >= DateTime.UtcNow.AddDays(-1))
			.ToList();
		return ValueTask.FromResult(new Immutable<ICollection<BaseGame>>(result));
	}
	private ValueTask NotifyAsync()
	{
		ObserverManager.Notify(obs => obs.ListStateChanged());
		return ValueTask.CompletedTask;
	}
}