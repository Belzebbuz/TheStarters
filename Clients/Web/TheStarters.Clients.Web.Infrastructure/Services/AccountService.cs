using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TheStarters.Clients.Web.Application.Abstractions.Services;
using TheStarters.Clients.Web.Application.Abstractions.Services.Mail;
using TheStarters.Clients.Web.Application.Settings.Auth;
using TheStarters.Clients.Web.Application.Wrapper;
using TheStarters.Clients.Web.Infrastructure.Constants;
using TheStarters.Clients.Web.Infrastructure.Identity.Models;
using Throw;

namespace TheStarters.Clients.Web.Infrastructure.Services;
internal class AccountService : IAccountService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SecuritySettings _securitySettings;
    private readonly IMailService _mailService;
    private readonly ICurrentUser _currentUser;
    private readonly string _webHookUrl;

    public AccountService(UserManager<AppUser> userManager,
                          SecuritySettings securitySettings,
                          IMailService mailService,
                          IConfiguration config,
                          ICurrentUser currentUser)
    {
        _userManager = userManager;
        _securitySettings = securitySettings;
        _mailService = mailService;
        _currentUser = currentUser;
        _webHookUrl = config["WebHookUrl"].ThrowIfNull();
    }

    public async Task<IResult> ChangePasswordAsync(ChangePasswordRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        user.ThrowIfNull(_ => new ValidationException($"User not found: {userId}"));

        var identityResult = await this._userManager.ChangePasswordAsync(
            user,
            request.OldPassword,
            request.NewPassword);
        var errors = identityResult.Errors.Select(e => e.Description.ToString()).ToList();
        return identityResult.Succeeded ? await Result.SuccessAsync() : await Result.FailAsync(errors);
    }

    public async Task<IResult> ConfirmAccountAsync(ConfirmAccountRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        user.ThrowIfNull(_ => new ValidationException($"User not found: {request.Email}"));
        if(user.ConfirmationToken == request.ConfirmToken)
        {
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            return Result.Success();
        }
        return Result.Fail("Incorrect confirm token");
    }

    public async Task<IResult> SelfRegisterAsync(SelfRegisterRequest request)
    {
        var existedUser = await _userManager.FindByEmailAsync(request.Email);
        if(existedUser != null && !existedUser.EmailConfirmed) 
        {
            await _userManager.DeleteAsync(existedUser);
        }
        var user = new AppUser()
        {
            UserName = request.Email,
            Email = request.Email,
            IsActive = true
        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return await Result.FailAsync(messages: GetErrors(result));

        var resultRole = await _userManager.AddToRoleAsync(user, Roles.User);
        if (!resultRole.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return await Result.FailAsync(messages: GetErrors(result));
        }
        if (_securitySettings.RequireConfirmedAccount)
        {
            user.ConfirmationToken = new Random().Next(99999, 1000000);
            await _userManager.UpdateAsync(user);
            var mail = new MailRequest(new() { user.Email },
                "zhendehanyu.ru Код подтверждение адреса электронной почты", 
                $"Код для подтверждения адреса - <h3>{user.ConfirmationToken}</h3>");
            await _mailService.SendAsync(mail, new());
        }
        return await Result.SuccessAsync();
    }

    public async Task<IResult> SendResetPasswordCallbackAsync(ResetPasswordCallbackRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        user.ThrowIfNull(_ => new ValidationException($"User not found: {request.Email}"));
        var restoreToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var uriToken = Uri.EscapeDataString(restoreToken);
        var callbackUrl = $"{_webHookUrl}/reset-password/{uriToken}/{user.Id}";
        var mail = new MailRequest(new() { user.Email },
                "zhendehanyu.ru Восстановление пароля",
                $"Для смены пароля перейдите по ссылке - <a href=\"{callbackUrl}\">Ссылка</a>");
        await _mailService.SendAsync(mail, new());
        return Result.Success();
    }
    public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        user.ThrowIfNull(_ => new ValidationException($"User not found: {request.UserId}"));
        var result = await _userManager.ResetPasswordAsync(user, request.ResetToken, request.NewPassword);
        if(!result.Succeeded)
            return Result.Fail(GetErrors(result));
        return Result.Success();
    }
    private List<string> GetErrors(IdentityResult result) =>
        result.Errors.Select(e => e.Description).ToList();
}
