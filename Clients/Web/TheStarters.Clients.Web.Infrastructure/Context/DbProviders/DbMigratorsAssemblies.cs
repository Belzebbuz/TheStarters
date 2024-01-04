namespace TheStarters.Clients.Web.Infrastructure.Context.DbProviders;
public class DbMigratorsAssemblies
{
    public DbMigratorsAssemblies(string postgreSQL, string sQLServer)
    {
        PostgreSQL = postgreSQL;
        SQLServer = sQLServer;
    }

    public string PostgreSQL { get; private set; }
    public string SQLServer { get; private set; }
}
