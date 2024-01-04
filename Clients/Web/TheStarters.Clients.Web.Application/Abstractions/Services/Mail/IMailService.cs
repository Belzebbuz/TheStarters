using TheStarters.Clients.Web.Application.Abstractions.DI;

namespace TheStarters.Clients.Web.Application.Abstractions.Services.Mail;
public interface IMailService : ITransientService
{
    Task SendAsync(MailRequest request, CancellationToken ct);
}
