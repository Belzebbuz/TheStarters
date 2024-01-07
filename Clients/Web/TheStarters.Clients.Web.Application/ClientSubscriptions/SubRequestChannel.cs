using System.Threading.Channels;

namespace TheStarters.Clients.Web.Application.ClientSubscriptions;

public sealed class SubRequestChannel
{
	public readonly Channel<ISubRequest> Requests = Channel.CreateUnbounded<ISubRequest>();
}