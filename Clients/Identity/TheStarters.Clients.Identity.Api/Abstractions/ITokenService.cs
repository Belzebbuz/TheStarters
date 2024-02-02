using ErrorOr;
using TheStarters.Client.Common.Abstractions.DI;

namespace TheStarters.Clients.Identity.Api.Abstractions;
public interface ITokenService : IScopedService
{
    Task<ErrorOr<TokenResponse>> GetTokenAsync(string email, string password, string? ipAddress, string userAgent);
    Task<ErrorOr<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress, string userAgent);
}

public record struct TokenRequest(string Email, string Password);
public record struct TokenResponse(string AccessToken, string RefreshToken);
public record struct RefreshTokenRequest(string AccessToken, string RefreshToken);