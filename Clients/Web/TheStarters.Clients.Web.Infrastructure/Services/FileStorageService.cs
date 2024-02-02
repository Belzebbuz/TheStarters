using ErrorOr;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TheStarters.Client.Common.Abstractions;
using TheStarters.Clients.Web.Application.Abstractions.Services;

namespace TheStarters.Clients.Web.Infrastructure.Services;

public class FileStorageService(
    IWebHostEnvironment hostEnvironment,
    ILogger<FileStorageService> logger,
    IHttpClientFactory clientFactory)
    : IFileStorageService
{
    private readonly string _rootPath = hostEnvironment.WebRootPath;
    private readonly HttpClient _client = clientFactory.CreateClient();

    public ErrorOr<Success> Delete(List<string> files)
    {
        foreach (var file in files)
        {
            var fullPath = Path.Combine(_rootPath, file);
            if(File.Exists(fullPath))
                File.Delete(fullPath);
        }
        return Result.Success;
    }

    public async Task<ErrorOr<string>> UploadAsync<T>(IFormFile file, CancellationToken cancellationToken = default)
        where T : class
    {
        try
        {
            var fileFolderPath = Path.Combine(_rootPath, typeof(T).Name);
            if (!Directory.Exists(fileFolderPath))
                Directory.CreateDirectory(fileFolderPath);
            var fileName = Path.Combine(typeof(T).Name, Guid.NewGuid() + Path.GetExtension(file.FileName));
            var filePath = Path.Combine(_rootPath, fileName);
            await using var fileStream = File.OpenWrite(filePath);
            await file.CopyToAsync(fileStream, cancellationToken);
            return fileName;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<string>> DownloadAsync<T>(Uri uri, string fileExtension) 
        where T : class
    {
        try
        {
            var file = await _client.GetByteArrayAsync(uri);
            var fileFolderPath = Path.Combine(_rootPath, typeof(T).Name);
            if (!Directory.Exists(fileFolderPath))
                Directory.CreateDirectory(fileFolderPath);
            var fileName = Path.Combine(typeof(T).Name, Guid.NewGuid() + fileExtension);
            var filePath = Path.Combine(_rootPath, fileName);
            await File.WriteAllBytesAsync(filePath, file);
            return fileName;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return Error.Failure(description: ex.Message);
        }
    }
}