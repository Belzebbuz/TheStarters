using TheStarters.Clients.Web.Application.Abstractions.DI;
using TheStarters.Clients.Web.Application.Wrapper;

namespace TheStarters.Clients.Web.Application.Abstractions.Services;
public interface ITokenService : IScopedService
{
    public Task<IResult<TokenResponse>> GetTokenAsync(string email, string password, string? ipAddress, string? userAgent);
    Task<IResult<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, string? ipAddress, string? userAgent);
}

public record struct TokenRequest(string Email, string Password);
public record struct TokenResponse(string AccessToken, string RefreshToken);
public record struct RefreshTokenRequest(string AccessToken, string RefreshToken);