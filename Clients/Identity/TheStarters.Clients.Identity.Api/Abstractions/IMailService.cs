using TheStarters.Client.Common.Abstractions.DI;

namespace TheStarters.Clients.Identity.Api.Abstractions;
public interface IMailService : ITransientService
{
    Task SendAsync(MailRequest request, CancellationToken ct);
}
