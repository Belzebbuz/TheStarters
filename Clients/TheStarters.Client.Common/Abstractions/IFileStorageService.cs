using ErrorOr;
using Microsoft.AspNetCore.Http;
using TheStarters.Client.Common.Abstractions.DI;

namespace TheStarters.Client.Common.Abstractions;

public interface IFileStorageService : ITransientService
{
	public Task<ErrorOr<string>> UploadAsync<T>(IFormFile file, CancellationToken cancellationToken = default)
		where T : class;

	Task<ErrorOr<string>> DownloadAsync<T>(Uri uri, string fileExtension) where T : class;
	public ErrorOr<Success> Delete(List<string> files);
}