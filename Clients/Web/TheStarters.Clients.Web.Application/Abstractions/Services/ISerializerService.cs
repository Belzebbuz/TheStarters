using TheStarters.Clients.Web.Application.Abstractions.DI;

namespace TheStarters.Clients.Web.Application.Abstractions.Services;

public interface ISerializerService : ITransientService
{
	public string Serialize<T>(T obj);
	public string Serialize<T>(T obj, Type type);
	public T? Deserialize<T>(string text);
}
