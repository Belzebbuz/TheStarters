namespace TheStarters.Server.Silo.Settings;

public class SiloSettings
{
	public required ClusterSettings ClusterSettings { get; set; }
	public required RedisPersistence RedisPersistence { get; set; }
}