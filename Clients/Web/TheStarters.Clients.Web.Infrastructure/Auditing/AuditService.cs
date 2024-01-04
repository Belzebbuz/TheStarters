using Mapster;
using Microsoft.EntityFrameworkCore;
using TheStarters.Clients.Web.Application.Abstractions.Services;
using TheStarters.Clients.Web.Infrastructure.Context;

namespace TheStarters.Clients.Web.Infrastructure.Auditing;
internal class AuditService : IAuditService
{
    private readonly AppDbContext _context;

    public AuditService(AppDbContext context) => _context = context;

    public async Task<List<AuditDto>> GetUserTrailsAsync(Guid userId)
    {
        var trails = await _context.AuditTrails
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.DateTime)
            .Take(250)
            .ToListAsync();

        return trails.Adapt<List<AuditDto>>();
    }
}
