using TheStarters.Server.Abstractions.Models;

namespace TheStarters.Clients.Web.Application.ClientSubscriptions;

public interface ISubRequest
{
}
public sealed record GameSubscribe(GameType GameType, long GameId) : ISubRequest;
public sealed record GameUnsubscribe(GameType GameType, long GameId) : ISubRequest;