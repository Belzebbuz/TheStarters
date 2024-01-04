using TheStarters.Clients.Web.Application.Abstractions.DI;
using TheStarters.Clients.Web.Domain.Common.Contracts;

namespace TheStarters.Clients.Web.Application.Abstractions.Services;

public interface IEventPublisher : ITransientService
{

	public Task PublishAsync(IEvent @event);
}
