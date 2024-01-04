using Orleans.Runtime;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Grains.Consts;

namespace TheStarters.Server.Grains;

public class GamesGrain : Grain, IGamesGrain
{
	private readonly IPersistentState<List<BaseGame>> _createdGames;

	public GamesGrain(
		[PersistentState(stateName:"created-TicTacToe", storageName: StorageConsts.PersistenceStorage)] 
		IPersistentState<List<BaseGame>> createdGames)
	{
		_createdGames = createdGames;
	}

	public ValueTask<List<BaseGame>> GetCreatedAsync() 
		=> ValueTask.FromResult(_createdGames.State.OrderByDescending(x => x.CreatedOn).ToList());

	public async ValueTask AddGameAsync(BaseGame game)
	{
		_createdGames.State.Add(game);
		await _createdGames.WriteStateAsync();
	}
}