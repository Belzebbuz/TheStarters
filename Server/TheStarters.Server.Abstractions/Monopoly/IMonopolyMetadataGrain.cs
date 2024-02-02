using Orleans.Concurrency;
using TheStarters.Server.Abstractions.Monopoly.Models;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;

namespace TheStarters.Server.Abstractions.Monopoly;

public interface IMonopolyMetadataGrain : IGrainWithGuidKey
{
	ValueTask<Dictionary<byte, MonopolyLand>> ConfigureBoardAsync();
}