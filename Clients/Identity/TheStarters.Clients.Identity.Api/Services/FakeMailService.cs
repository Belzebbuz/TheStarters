using TheStarters.Clients.Identity.Api.Abstractions;

namespace TheStarters.Clients.Identity.Api.Services;

public class FakeMailService : IMailService
{
	public Task SendAsync(MailRequest request, CancellationToken ct)
	{
		return Task.CompletedTask;
	}
}