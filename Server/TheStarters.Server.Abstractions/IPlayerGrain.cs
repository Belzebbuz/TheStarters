﻿using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Server.Abstractions;

public interface IPlayerGrain : IGrainWithGuidKey
{
	ValueTask<PlayerProfile> GetProfileAsync();
	ValueTask UpdateProfileAsync(PlayerProfile playerProfile);
	ValueTask AddToGameAsync(long gameId);
	ValueTask RemoveFromGameAsync(long gameId);
}