using Microsoft.EntityFrameworkCore;

namespace TheStarters.Clients.Web.Infrastructure.Context.DbProviders;
public static class Extensions
{
    public static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder,
        string dbProvider, string connectionString, DbMigratorsAssemblies dbMigrators)
    {
        switch (dbProvider)
        {
            case DbProviderKeys.PostgreSQL:
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                return builder.UseNpgsql(connectionString, e =>
                    e.MigrationsAssembly(dbMigrators.PostgreSQL));
            default:
                throw new InvalidOperationException($"DB Provider {dbProvider} is not supported.");
        }
    }
}
