using Microsoft.EntityFrameworkCore;
using Throw;

namespace TheStarters.Clients.Identity.Api.Context;

internal static class Extensions
{
    public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration config)
    {
        var dbSettings = GetDbSettings(config);
        return services
            .AddTransient<DbSeeder>()
            .AddSingleton(dbSettings)
            .AddDbContext<AppDbContext>(m => m.UseDatabase(dbSettings.ConnectionString!));
    }

    public static async Task InitDatabaseAsync<T>(this IApplicationBuilder app) where T : DbContext
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetService<T>();
        await context!.Database.MigrateAsync();
        await scope.ServiceProvider.GetService<DbSeeder>()!.SeedDataAsync();
    }

    private static DatabaseSettings GetDbSettings(IConfiguration config)
    {
        var dbSettings = config.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        dbSettings.ThrowIfNull()
            .IfNullOrEmpty(x => x.ConnectionString)
            .IfNullOrEmpty(x => x.Provider);
        return dbSettings;
    }

    public static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder,string connectionString)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        return builder.UseNpgsql(connectionString);
    }
}
