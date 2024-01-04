namespace TheStarters.Clients.Web.Application.Settings.Orleans;

public class OrleansSettings
{
	public required string ConnectionString { get; set; }
	public required string ClusterId { get; set; }
	public required string ServiceId { get; set; }
}