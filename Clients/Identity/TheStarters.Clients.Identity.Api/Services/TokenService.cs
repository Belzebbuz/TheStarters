﻿using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TheStarters.Clients.Identity.Api.Abstractions;
using TheStarters.Clients.Identity.Api.Options;
using TheStarters.Clients.Identity.Api.Services.Identity.Models;
using Throw;

namespace TheStarters.Clients.Identity.Api.Services;

public class TokenService : ITokenService
{
    private readonly SecuritySettings _securitySettings;
    private readonly UserManager<AppUser> _userManager;
    public TokenService(
        UserManager<AppUser> userManager,
        SecuritySettings jwtSettings)
    {
        _userManager = userManager;
        _securitySettings = jwtSettings;
    }
    public async Task<ErrorOr<TokenResponse>> GetTokenAsync(string email, string password, string? ipAddress, string userAgent)
    {
        var user = await _userManager.FindByEmailAsync(email.Trim().Normalize());
        user
            .ThrowIfNull(_ => new ValidationException($"User not found {email}"))
            .Throw(_ => new ValidationException("User not active"))
            .IfFalse(x => x.IsActive)
            .Throw(_ => new ValidationException("Require email confirmation"))
            .IfTrue(_securitySettings.RequireConfirmedAccount && !user.EmailConfirmed);

        if (!await _userManager.CheckPasswordAsync(user, password))
            return Error.Validation("Invalid email or password");
        
        return await GenerateTokensAndUpdateUser(user, ipAddress, userAgent);
    }
    public async Task<ErrorOr<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress, string userAgent)
    {
        var userPrincipal = GetPrincipalFromExpiredToken(request.AccessToken);
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(userEmail!);
        user
           .ThrowIfNull(_ => new ValidationException($"User not found {userEmail}"))
           .Throw(_ => new ValidationException("User not active"))
           .IfFalse(x => x.IsActive)
           .Throw(_ => new ValidationException("Require email confirmation"))
           .IfTrue(_securitySettings.RequireConfirmedAccount && !user.EmailConfirmed);
        user.RefreshTokens.ThrowIfNull();
        if (user.RefreshTokens[userAgent] != request.RefreshToken)
            return Error.Validation("Invalid refresh token");
        user.RefreshTokens.Remove(request.RefreshToken);
        return await GenerateTokensAndUpdateUser(user, ipAddress, userAgent);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
    {
        var key = Encoding.UTF8.GetBytes(_securitySettings.JwtSettings.Key);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateLifetime = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new UnauthorizedAccessException("Invalid Token.");
        }

        return principal;
    }

    private async Task<TokenResponse> GenerateTokensAndUpdateUser(AppUser user, string? ipAddress, string userAgent)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwt(user, ipAddress, roles);
        var refreshToken = GenerateRefreshToken();
        user.RefreshTokens ??= new();
        user.RefreshTokens[userAgent] = refreshToken;
        await _userManager.UpdateAsync(user);
        return new(token, refreshToken);
    }
    private static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GenerateJwt(AppUser user, string? ipAddress, IList<string> roles) =>
      GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user, ipAddress, roles));

    private IEnumerable<Claim> GetClaims(AppUser user, string? ipAddress, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email.ThrowIfNull(nameof(user.Email))),
        };

        if(ipAddress is not null)
            claims.Add( new("ipAddress", ipAddress));

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_securitySettings.JwtSettings.ExpirationInMinutes),
            signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private SigningCredentials GetSigningCredentials()
    {
        if (string.IsNullOrEmpty(_securitySettings.JwtSettings.Key))
            throw new InvalidOperationException("No Key defined in JwtSettings config.");

        byte[] secret = Encoding.UTF8.GetBytes(_securitySettings.JwtSettings.Key);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
}

