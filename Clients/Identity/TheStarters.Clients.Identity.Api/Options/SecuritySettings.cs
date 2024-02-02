namespace TheStarters.Clients.Identity.Api.Options;

public class SecuritySettings
{
	public required JwtSettings JwtSettings { get; set; }
	public required string RootUserEmail { get; set; }
	public required string DefaultPassword { get; set; }
	public bool RequireConfirmedAccount { get; set; }
}
public class JwtSettings
{
	public required string Key { get; set; }
	public int ExpirationInMinutes { get; set; }
	public int RefreshTokenExpirationInDays { get; set; }
}
