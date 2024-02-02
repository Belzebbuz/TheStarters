using Orleans.TestingHost;
using Shouldly;
using TheStarters.Server.Abstractions;
using TheStarters.Server.Abstractions.Models;
using InvalidOperationException = System.InvalidOperationException;

namespace TheStarters.Server.Tests.TicTacToe;

[Collection(ClusterCollection.Name)]
public class TicTacToeHostedTests(ClusterFixture fixture)
{
	private readonly TestCluster _cluster = fixture.Cluster;
	private readonly Guid _playerId1 = Guid.NewGuid();
	private readonly Guid _playerId2 = Guid.NewGuid();
	[Fact]
	public async Task CreateGame_ShouldBe_Ok()
	{
		var gameId = await CreateGameAsync(_playerId1);

		var gameGrain = _cluster.GrainFactory.GetGrain<ITicTacToeGrain>(gameId);
		var game = await gameGrain.GetAsync();
		
		game.GameType.ShouldBe(GameType.TicTacToe);
		game.Player1.ShouldBe(_playerId1);
		game.CurrentPlayer.HasValue.ShouldBeFalse();
	}
	
	[Fact]
	public async Task StartGame_OnePlayer_ShouldBeError()
	{
		var gameId = await CreateGameAsync(_playerId1);

		var gameGrain = _cluster.GrainFactory.GetGrain<ITicTacToeGrain>(gameId);
		await Should.ThrowAsync<InvalidOperationException>(async () => await gameGrain.StartAsync(_playerId1));
	}
	
	[Fact]
	public async Task StartGame_TwoPlayers_ShouldBeOk()
	{
		var gameId = await StartGameAsync();

		var gameGrain = _cluster.GrainFactory.GetGrain<ITicTacToeGrain>(gameId);
		
		var game = await gameGrain.GetAsync();
		game.CurrentPlayer.ShouldBe(_playerId1);
		game.Started.ShouldBe(true);
		game.PlayersCount.ShouldBe((byte)2);
	}

	[Fact]
	public async Task SetAnswer_TwoTimes_ShouldThrow()
	{
		var gameId = await StartGameAsync();
		var grain = _cluster.GrainFactory.GetGrain<ITicTacToeGrain>(gameId);
		await grain.SetAnswerAsync(_playerId1, 0, 0);
		await Should.ThrowAsync<Exception>(async () => await grain.SetAnswerAsync(_playerId1, 0, 1));
	}

	[Fact]
	public async Task SetAnswers_InOneRow_WinnerShouldBeSet()
	{
		var gameId = await StartGameAsync();
		var grain = _cluster.GrainFactory.GetGrain<ITicTacToeGrain>(gameId);
		await grain.SetAnswerAsync(_playerId1, 0, 0);
		await grain.SetAnswerAsync(_playerId2, 1, 0);
		await grain.SetAnswerAsync(_playerId1, 0, 1);
		await grain.SetAnswerAsync(_playerId2, 1, 1);
		await grain.SetAnswerAsync(_playerId1, 0, 2);
		var game = await grain.GetAsync();
		game.Winner.HasValue.ShouldBeTrue();
		game.Winner!.Value.ShouldBe(_playerId1);
	}
	
	[Fact]
	public async Task SetAnswer_SamePosition_ShouldThrow()
	{
		var gameId = await StartGameAsync();
		var grain = _cluster.GrainFactory.GetGrain<ITicTacToeGrain>(gameId);
		await grain.SetAnswerAsync(_playerId1, 0, 0);
		await Should.ThrowAsync<Exception>(async () => await grain.SetAnswerAsync(_playerId2, 0, 0));
	}
	
	private async Task<long> CreateGameAsync(Guid playerId)
	{
		var gameFactoryGrain = _cluster.GrainFactory.GetGrain<IGameFactoryGrain>(Guid.Empty);

		return await gameFactoryGrain.CreateGameAsync(GameType.TicTacToe, playerId);
	}

	private async Task<long> StartGameAsync()
	{
		var gameId = await CreateGameAsync(_playerId1);
		var gameGrain = _cluster.GrainFactory.GetGrain<ITicTacToeGrain>(gameId);
		await gameGrain.AddPlayerAsync(_playerId2);
		await gameGrain.StartAsync(_playerId1);
		return gameId;
	}
}