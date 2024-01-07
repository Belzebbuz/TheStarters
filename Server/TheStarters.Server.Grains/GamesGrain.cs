using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Runtime;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Grains.Consts;

namespace TheStarters.Server.Grains;

public class GamesGrain :  IGamesGrain
{
	private readonly IPersistentState<ConcurrentDictionary<long,BaseGame>> _createdGames;
	private readonly ILogger<GamesGrain> _logger;
	
	public GamesGrain(
		[PersistentState(stateName:"created-TicTacToe", storageName: StorageConsts.PersistenceStorage)] 
		IPersistentState<ConcurrentDictionary<long,BaseGame>> createdGames,
		ILogger<GamesGrain> logger)
	{
		_createdGames = createdGames;
		_logger = logger;
	}

	public ValueTask<Immutable<ICollection<BaseGame>>> GetCreatedAsync()
	{
		var result = _createdGames.State.Values.OrderByDescending(x => x.CreatedOn).ToList();
		return ValueTask.FromResult(
			new Immutable<ICollection<BaseGame>>(result));
	}

	public async ValueTask AddOrUpdateGameAsync(BaseGame game)
	{
		_createdGames.State[game.Id] = game;
		await _createdGames.WriteStateAsync();
	}

	public ValueTask<Immutable<ICollection<BaseGame>>> GetLastUpdatedAsync()
	{
		var result = _createdGames.State.Values
			.Where(x => !x.Winner.HasValue)
			.Where(x => x.LastUpdateOn >= DateTime.UtcNow.AddDays(-1))
			.ToList();
		return ValueTask.FromResult(new Immutable<ICollection<BaseGame>>(result));
	}
}