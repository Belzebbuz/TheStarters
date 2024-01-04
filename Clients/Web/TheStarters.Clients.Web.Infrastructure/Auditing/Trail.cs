using TheStarters.Clients.Web.Domain.Common.Base;

namespace TheStarters.Clients.Web.Infrastructure.Auditing;
public class Trail : BaseEntity<Guid>
{
    public Guid UserId { get; set; }
    public string? Type { get; set; }
    public string? TableName { get; set; }
    public DateTime DateTime { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }
    public string? PrimaryKey { get; set; }
}