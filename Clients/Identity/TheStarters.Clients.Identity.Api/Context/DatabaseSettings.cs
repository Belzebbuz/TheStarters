namespace TheStarters.Clients.Identity.Api.Context;

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public bool SeedTestData { get; set; } = false;
}
