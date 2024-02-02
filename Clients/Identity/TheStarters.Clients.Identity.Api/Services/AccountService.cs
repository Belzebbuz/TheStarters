using ErrorOr;
using Mapster;
using Microsoft.AspNetCore.Identity;
using TheStarters.Client.Common.Abstractions;
using TheStarters.Clients.Identity.Api.Abstractions;
using TheStarters.Clients.Identity.Api.Constants;
using TheStarters.Clients.Identity.Api.Options;
using TheStarters.Clients.Identity.Api.Services.Identity.Models;
using Throw;

namespace TheStarters.Clients.Identity.Api.Services;
internal class AccountService(
    UserManager<AppUser> userManager,
    SecuritySettings securitySettings,
    IMailService mailService,
    IConfiguration config,
    ICurrentUser currentUser,
    IFileStorageService service)
    : IAccountService
{
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IFileStorageService _service = service;
    private readonly string _webHookUrl = config["WebHookUrl"].ThrowIfNull();

    public async Task<ErrorOr<Success>> ChangePasswordAsync(ChangePasswordRequest request, string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if(user is null)
            return Error.Failure(description: $"Пользователь не найден: {userId}");

        var identityResult = await userManager.ChangePasswordAsync(
            user,
            request.OldPassword,
            request.NewPassword);
        var errors = identityResult
            .Errors.Select(e => Error.Validation(description: e.Description)).ToList();
        return identityResult.Succeeded ? Result.Success : errors;
    }

    public async Task<ErrorOr<Success>> ConfirmAccountAsync(ConfirmAccountRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if(user is null)
            return Error.Failure(description: $"Пользователь не найден: {request.Email}");
        if (user.ConfirmationToken != request.ConfirmToken)
            return Error.Validation(description: "Incorrect confirm token");
        
        user.EmailConfirmed = true;
        await userManager.UpdateAsync(user);
        return Result.Success;
    }

    public async Task<ErrorOr<SelfRegisterResponse>> SelfRegisterAsync(SelfRegisterRequest request)
    {
        var existedUser = await userManager.FindByEmailAsync(request.Email);
        if(existedUser != null && !existedUser.EmailConfirmed) 
        {
            await userManager.DeleteAsync(existedUser);
        }
        var user = new AppUser()
        {
            UserName = request.Name,
            Email = request.Email,
            IsActive = true
        };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return GetErrors(result).Select(x => Error.Validation(description: x)).ToList();

        var resultRole = await userManager.AddToRoleAsync(user, Roles.User);
        if (!resultRole.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return GetErrors(resultRole).Select(x => Error.Validation(description: x)).ToList();
        }

        if (!securitySettings.RequireConfirmedAccount) return user.Adapt<SelfRegisterResponse>();
        
        user.ConfirmationToken = new Random().Next(99999, 1000000);
        await userManager.UpdateAsync(user);
        
        var mail = new MailRequest(new() { user.Email },
            "zhendehanyu.ru Код подтверждения адреса электронной почты", 
            $"Код для подтверждения адреса - <h3>{user.ConfirmationToken}</h3>");
        await mailService.SendAsync(mail, new());
        return user.Adapt<SelfRegisterResponse>();
    }

    public async Task<ErrorOr<Success>> SendResetPasswordCallbackAsync(ResetPasswordCallbackRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if(user is null)
            return Error.Failure(description: $"Пользователь не найден: {request.Email}");
        var restoreToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var uriToken = Uri.EscapeDataString(restoreToken);
        var callbackUrl = $"{_webHookUrl}/reset-password/{uriToken}/{user.Id}";
        var mail = new MailRequest(new() { user.Email.ThrowIfNull(nameof(user.Email)) },
                "zhendehanyu.ru Восстановление пароля",
                $"Для смены пароля перейдите по ссылке - <a href=\"{callbackUrl}\">Ссылка</a>");
        await mailService.SendAsync(mail, new());
        return Result.Success;
    }
    public async Task<ErrorOr<Success>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if(user is null)
            return Error.Failure(description: $"Пользователь не найден: {request.UserId}");

        var result = await userManager.ResetPasswordAsync(user, request.ResetToken, request.NewPassword);
        if(!result.Succeeded)
            return GetErrors(result).Select(x => Error.Failure(description: x)).ToList();
        return Result.Success;
    }
    private static IEnumerable<string> GetErrors(IdentityResult result) =>
        result.Errors.Select(e => e.Description).ToList();
}
