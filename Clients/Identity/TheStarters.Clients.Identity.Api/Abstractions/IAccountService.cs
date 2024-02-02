using ErrorOr;
using TheStarters.Client.Common.Abstractions.DI;

namespace TheStarters.Clients.Identity.Api.Abstractions;
public interface IAccountService : IScopedService
{
    public Task<ErrorOr<SelfRegisterResponse>> SelfRegisterAsync(SelfRegisterRequest request);
    public Task<ErrorOr<Success>> ChangePasswordAsync(ChangePasswordRequest request, string userId);
    public Task<ErrorOr<Success>> ConfirmAccountAsync(ConfirmAccountRequest request);
    public Task<ErrorOr<Success>> SendResetPasswordCallbackAsync(ResetPasswordCallbackRequest request);
    public Task<ErrorOr<Success>> ResetPasswordAsync(ResetPasswordRequest request);
}

public record struct SelfRegisterRequest(string Password, string Email, string Name);
public record struct SelfRegisterResponse(string Id, string Email, string UserName);
public record struct ChangePasswordRequest(string NewPassword, string OldPassword);
public record struct ConfirmAccountRequest(int ConfirmToken, string Email);
public record struct ResetPasswordCallbackRequest(string Email);
public record struct ResetPasswordRequest(string ResetToken, string UserId, string NewPassword);
