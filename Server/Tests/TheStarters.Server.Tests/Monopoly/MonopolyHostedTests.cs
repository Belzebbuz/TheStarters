using Orleans.TestingHost;
using Shouldly;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Abstractions.Monopoly;
using TheStarters.Server.Abstractions.Monopoly.Models.Commands;

namespace TheStarters.Server.Tests.Monopoly;

[Collection(ClusterCollection.Name)]
public class MonopolyHostedTests(ClusterFixture fixture)
{
	private readonly TestCluster _cluster = fixture.Cluster;
	private readonly List<Guid> _players = [Guid.NewGuid(), Guid.NewGuid()];
	
	[Fact]
	public async Task CreateGame_ShouldBe_Ok()
	{
		await CreatePlayersAsync();
		var gameId = await CreateGameAsync(_players[0]);

		var gameGrain = _cluster.GrainFactory.GetGrain<IMonopolyGrain>(gameId);
		var game = await gameGrain.GetAsync();
		
		game.GameType.ShouldBe(GameType.Monopoly);
		game.Players.Keys.ShouldContain(_players[0]);
		game.CurrentPlayerId.HasValue.ShouldBeFalse();
	}
	[Fact]
	public async Task AddPlayer_ShouldBe_Ok()
	{
		await CreatePlayersAsync();
		var gameId = await CreateGameAsync(_players[0]);

		var gameGrain = _cluster.GrainFactory.GetGrain<IMonopolyGrain>(gameId);
		await gameGrain.AddPlayerAsync(_players[1]);
		
		var game = await gameGrain.GetAsync();
		game.Players.Keys.ShouldContain(_players[1]);
		game.CurrentPlayerId.HasValue.ShouldBeFalse();
	}
	
	[Fact]
	public async Task StartGame_ShouldBe_Ok()
	{
		var gameId = await StartGameAsync();

		var gameGrain = _cluster.GrainFactory.GetGrain<IMonopolyGrain>(gameId);
		
		var game = await gameGrain.GetAsync();
		
		game.CurrentPlayerId.HasValue.ShouldBeTrue();
		game.Players[game.CurrentPlayerId!.Value].Commands.Values.OfType<MoveCommand>().Count().ShouldBe(1);
	}
	
	[Fact]
	public async Task FirstTurn_ShouldAddBuyCommand()
	{
		var gameId = await StartGameAsync();

		var grain = _cluster.GrainFactory.GetGrain<IMonopolyGrain>(gameId);
		
		var game = await grain.GetAsync();
		var currentPlayerId = game.CurrentPlayerId!.Value;
		var moveCommandId = game.Players[currentPlayerId].Commands.First().Value.Id;
		await grain.ExecuteCommandAsync(currentPlayerId, moveCommandId);
		game = await grain.GetAsync();
		game.Players[currentPlayerId].LandId.ShouldBe((byte)(game.DicePair.Total + 1));
	}
	
	private async Task CreatePlayersAsync()
	{
		foreach (var playerId in _players)
		{
			var playerGrain = _cluster.GrainFactory.GetGrain<IPlayerGrain>(playerId);
			await playerGrain.SetNameAsync(Faker.Name.First());
		}
	}

	private async Task<long> CreateGameAsync(Guid playerId)
	{
		var gameFactoryGrain = _cluster.GrainFactory.GetGrain<IGameFactoryGrain>(Guid.Empty);

		return await gameFactoryGrain.CreateGameAsync(GameType.Monopoly, playerId);
	}

	private async Task<long> StartGameAsync()
	{
		await CreatePlayersAsync();
		var gameId = await CreateGameAsync(_players[0]);
		var grain = _cluster.GrainFactory.GetGrain<IMonopolyGrain>(gameId);
		
		foreach (var playerId in _players.Skip(1))
		{
			await grain.AddPlayerAsync(playerId);
		}
		
		await grain.StartGameAsync(_players[0]);
		return gameId;
	}
}