using Microsoft.EntityFrameworkCore;
using TheStarters.Client.Common.Abstractions;
using TheStarters.Clients.Web.Application.Abstractions.Services;

namespace TheStarters.Clients.Web.Infrastructure.Context;

public class AppDbContext : BaseDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options,
                        ISerializerService serializerService,
                        ICurrentUser currentUser,
                        IEventPublisher eventPublisher) 
        : base(options, serializerService, currentUser, eventPublisher)
    {
    }
}
