using Orleans.Concurrency;
using TheStarters.Server.Abstractions.Models;
using TheStarters.Server.Abstractions.Monopoly.Models;
using TheStarters.Server.Abstractions.Monopoly.Models.Commands;

namespace TheStarters.Server.Abstractions.Monopoly;

public interface IMonopolyGrain : IGrainWithIntegerKey, IObservableGrain<IMonopolyObserver>
{
	Task<BaseGame> InitStateAsync(Guid userId);
	ValueTask<MonopolyGame> GetAsync();
	Task StartGameAsync(Guid userId);
	Task AddPlayerAsync(Guid userId);
	Task RemovePlayerAsync(Guid userId);
	Task ExecuteCommandAsync(Guid userId, int commandId);
	Task EndTurnAsync(Guid userId);
	Task ExecuteLandOperationAsync(Guid userId, byte landId, LandOperation operation);
	ValueTask<Immutable<List<MonopolyBuyRequest>>> GetOwnerBuyRequestsAsync(Guid userId);
	Task ConfirmBuyRequestAsync(Guid userId, Guid buyRequestId);
	Task CreateBuyLandRequestAsync(Guid userId, byte landId);
}

public interface IMonopolyObserver : IGrainObserver
{
	Task GameStateChanged();
	Task BuyRequestsChanged(Guid ownerId);
}