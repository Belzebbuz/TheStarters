using Microsoft.AspNetCore.Identity;

namespace TheStarters.Clients.Web.Infrastructure.Identity.Models;

public class AppUser : IdentityUser
{
	public bool IsActive { get; set; }
	public Dictionary<string, string>? RefreshTokens { get; set; }
	public int? ConfirmationToken { get; set; }
}