using TheStarters.Clients.Web.Domain.Common.Contracts;

namespace TheStarters.Clients.Web.Domain.Common.Base;

public class DomainEvent : IEvent
{
    public DateTime TriggeredOn { get; protected set; } = DateTime.UtcNow;
}