using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using TheStarters.Clients.Web.Application.Abstractions.Services;
using Throw;

namespace TheStarters.Clients.Web.Infrastructure.Services;

public class SystemJsonService : ISerializerService
{
	private readonly JsonSerializerOptions? _options = new JsonSerializerOptions()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
	};

	/// <inheritdoc/>
	public T? Deserialize<T>(string text)
	{
		return JsonSerializer.Deserialize<T>(text.ThrowIfNull(), _options);
	}

	/// <inheritdoc/>
	public string Serialize<T>(T obj)
	{
		return JsonSerializer.Serialize(obj, _options)
			.Throw().IfNullOrEmpty(value => value);
	}

	/// <inheritdoc/>
	public string Serialize<T>(T obj, Type type)
	{
		return JsonSerializer.Serialize(obj, type, _options)
			.Throw().IfNullOrEmpty(value => value);
	}
}
