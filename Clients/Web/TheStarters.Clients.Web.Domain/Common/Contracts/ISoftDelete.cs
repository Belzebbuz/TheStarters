﻿namespace TheStarters.Clients.Web.Domain.Common.Contracts;

public interface ISoftDelete
{
    DateTimeOffset? DeletedOn { get; set; }
    Guid? DeletedBy { get; set; }
}
