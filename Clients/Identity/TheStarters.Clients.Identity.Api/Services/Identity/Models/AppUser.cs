using Microsoft.AspNetCore.Identity;

namespace TheStarters.Clients.Identity.Api.Services.Identity.Models;

public class AppUser : IdentityUser
{
	public bool IsActive { get; set; }
	public Dictionary<string, string>? RefreshTokens { get; set; }
	public int? ConfirmationToken { get; set; }
	public string? ImageUrl { get; set; }
}