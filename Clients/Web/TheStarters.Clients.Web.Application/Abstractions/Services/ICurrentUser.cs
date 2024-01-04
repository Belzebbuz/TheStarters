﻿using System.Security.Claims;

namespace TheStarters.Clients.Web.Application.Abstractions.Services;

public interface ICurrentUser
{
	public string? Name { get; }

	public Guid GetUserId();

	public string? GetUserEmail();

	public bool IsAuthenticated();

	public IEnumerable<Claim>? GetUserClaims();
}

public interface ICurrentUserInitializer
{
	public void SetCurrentUser(ClaimsPrincipal user);

	public void SetCurrentUserId(string userId);
}