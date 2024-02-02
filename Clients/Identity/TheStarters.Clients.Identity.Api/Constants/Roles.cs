using System.Collections.ObjectModel;

namespace TheStarters.Clients.Identity.Api.Constants;

public static class Roles
{
	public const string Admin = nameof(Admin);
	public const string User = nameof(User);

	public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(new[]
	{
		Admin,
		User,
	});

	public static bool IsDefault(string roleName) => DefaultRoles.Any(r => r == roleName);
}