using TheStarters.Clients.Web.Application.Abstractions.DI;
using TheStarters.Clients.Web.Application.Wrapper;

namespace TheStarters.Clients.Web.Application.Abstractions.Services;
public interface IAccountService : IScopedService
{
    public Task<IResult> SelfRegisterAsync(SelfRegisterRequest request);
    public Task<IResult> ChangePasswordAsync(ChangePasswordRequest request, string userId);
    public Task<IResult> ConfirmAccountAsync(ConfirmAccountRequest request);
    public Task<IResult> SendResetPasswordCallbackAsync(ResetPasswordCallbackRequest request);
    public Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);
}

public record struct SelfRegisterRequest(string Password, string Email);
public record struct ChangePasswordRequest(string NewPassword, string OldPassword);
public record struct ConfirmAccountRequest(int ConfirmToken, string Email);
public record struct ResetPasswordCallbackRequest(string Email);
public record struct ResetPasswordRequest(string ResetToken, string UserId, string NewPassword);
