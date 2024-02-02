using System.Security.Claims;
using TheStarters.Client.Common.Abstractions;
using Throw;

namespace TheStarters.Client.Common.Middlewares.RequestCurrentUser;
internal class CurrentUser : ICurrentUser, ICurrentUserInitializer
{
    private ClaimsPrincipal? _user;

    public string? Name => _user?.Identity?.Name;

    private Guid _userId = Guid.Empty;
    public Guid GetUserId() =>
        IsAuthenticated()
            ? Guid.Parse(_user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString())
            : _userId;

    public string? GetUserEmail() =>
        IsAuthenticated()
            ? _user!.FindFirstValue(ClaimTypes.Email)
            : string.Empty;

    public bool IsAuthenticated() =>
        _user?.Identity?.IsAuthenticated is true;

    public IEnumerable<Claim>? GetUserClaims() =>
        _user?.Claims;
    public void SetCurrentUser(ClaimsPrincipal user)
    {
        _user = user.ThrowIfNull();
    }

    public void SetCurrentUserId(string userId)
    {
        if (_userId != Guid.Empty)
            throw new Exception("Method reserved for in-scope initialization");

        _userId = Guid.Parse(userId.Throw().IfNullOrEmpty(x => x));
    }
}
