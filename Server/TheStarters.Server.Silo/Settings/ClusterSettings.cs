namespace TheStarters.Server.Silo.Settings;

public class ClusterSettings
{
	public required string ConnectionString { get; set; }
	public required string ClusterId { get; set; }
	public required string ServiceId { get; set; }
}