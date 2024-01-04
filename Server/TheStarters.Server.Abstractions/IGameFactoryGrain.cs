using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Server.Abstractions;

public interface IGameFactoryGrain : IGrainWithGuidKey
{
	ValueTask<long> CreateGameAsync(GameType gameType, Guid userId);
}