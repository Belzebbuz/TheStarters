using TheStarters.Clients.Web.Application.Abstractions.Services;
using TheStarters.Clients.Web.Domain.Common.Contracts;

namespace TheStarters.Clients.Web.Infrastructure.Services.Events;

public class EventPublisher : IEventPublisher
{
	public Task PublishAsync(IEvent @event)
	{
		return Task.CompletedTask;
	}
}