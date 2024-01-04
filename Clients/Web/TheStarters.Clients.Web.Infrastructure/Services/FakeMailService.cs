using TheStarters.Clients.Web.Application.Abstractions.Services.Mail;

namespace TheStarters.Clients.Web.Infrastructure.Services;

public class FakeMailService : IMailService
{
	public Task SendAsync(MailRequest request, CancellationToken ct)
	{
		return Task.CompletedTask;
	}
}