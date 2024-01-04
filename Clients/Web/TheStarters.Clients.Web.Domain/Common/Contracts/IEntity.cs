using TheStarters.Clients.Web.Domain.Common.Base;

namespace TheStarters.Clients.Web.Domain.Common.Contracts;

public interface IEntity
{
    public List<DomainEvent> DomainEvents { get; }
}

public interface IEntity<TId> : IEntity
    where TId : struct
{
    TId Id { get; }
}
