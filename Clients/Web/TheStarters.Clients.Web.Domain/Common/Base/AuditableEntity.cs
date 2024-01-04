using TheStarters.Clients.Web.Domain.Common.Contracts;

namespace TheStarters.Clients.Web.Domain.Common.Base;

public abstract class AuditableEntity<TId> : BaseEntity<TId>, IAuditableEntity, ISoftDelete
    where TId : struct
{
    public Guid CreatedBy { get; set; }
    public DateTimeOffset CreatedOn { get; private set; }
    public DateTimeOffset? LastModifiedOn { get; set; }
    public Guid LastModifiedBy { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }

    protected AuditableEntity()
    {
        CreatedOn = DateTimeOffset.UtcNow;
        LastModifiedOn = DateTimeOffset.UtcNow;
    }
    public void MarkAsDeleted(Guid userId)
    {
        DeletedOn = DateTimeOffset.UtcNow;
        DeletedBy = userId;
    }
}
