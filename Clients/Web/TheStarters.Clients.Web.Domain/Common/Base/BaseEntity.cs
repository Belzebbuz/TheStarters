using System.ComponentModel.DataAnnotations.Schema;
using TheStarters.Clients.Web.Domain.Common.Contracts;

namespace TheStarters.Clients.Web.Domain.Common.Base;

public abstract class BaseEntity<TId> : IEntity<TId>
    where TId : struct
{
    public TId Id { get; protected set; } = default!;

    [NotMapped]
    public List<DomainEvent> DomainEvents { get; } = new();

    #region Equals
    //public bool Equals(BaseEntity<TId>? other)
    //{
    //	return Equals((object?)other);
    //}

    //public override bool Equals(object? obj)
    //{
    //	return obj is BaseEntity<TId> entity && Id.Equals(entity.Id) ;
    //}

    //public override int GetHashCode()
    //{
    //	return Id.GetHashCode();
    //}

    //public static bool operator ==(BaseEntity<TId> left, BaseEntity<TId> right)
    //{
    //	return Equals(left, right);
    //}
    //public static bool operator !=(BaseEntity<TId> left, BaseEntity<TId> right)
    //{
    //	return !Equals(left, right);
    //}

    #endregion
}
