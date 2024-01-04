using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheStarters.Clients.Web.Application.Abstractions.Repository;
using TheStarters.Clients.Web.Domain.Common.Contracts;
using TheStarters.Clients.Web.Infrastructure.Context.DbProviders;
using Throw;

namespace TheStarters.Clients.Web.Infrastructure.Context;

internal static class Extensions
{
    public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration config)
    {
        var dbSettings = GetDbSettings(config);
        return services
            .AddTransient<DbSeeder>()
            .AddRepositories()
            .AddSingleton(dbSettings)
            .AddDbContext<AppDbContext>(m => m.UseDatabase(dbSettings.Provider!, dbSettings.ConnectionString!));
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
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(ApplicationDbRepository<>));
        foreach (var aggregateRootType in
                 typeof(IEntity<>).Assembly.GetExportedTypes()
                     .Where(t => typeof(IEntity).IsAssignableFrom(t) && t.IsClass)
                     .ToList())
            services.AddScoped(typeof(IReadRepository<>).MakeGenericType(aggregateRootType), sp =>
                sp.GetRequiredService(typeof(IRepository<>).MakeGenericType(aggregateRootType)));
        return services;
    }
    public static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder,
        string dbProvider, string connectionString)
    {
        switch (dbProvider)
        {
            case DbProviderKeys.PostgreSQL:
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                return builder.UseNpgsql(connectionString);
            default:
                throw new InvalidOperationException($"DB Provider {dbProvider} is not supported.");
        }
    }
}
