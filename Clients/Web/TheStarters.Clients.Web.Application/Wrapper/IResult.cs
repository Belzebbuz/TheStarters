using System.Text.Json.Serialization;

namespace TheStarters.Clients.Web.Application.Wrapper;

public interface IResult
{
    [JsonPropertyName("messages")]
    List<string> Messages { get; set; }

    [JsonPropertyName("succeded")]
    bool Succeeded { get; set; }
    public static bool operator !(IResult result) => !result.Succeeded;
    public static bool operator true(IResult result) => result.Succeeded;
    public static bool operator false(IResult result) => !result.Succeeded;
}

public interface IResult<out T> : IResult
{
    [JsonPropertyName("data")]
    T Data { get; }
    public static bool operator true(IResult<T> result) => result.Succeeded;
    public static bool operator false(IResult<T> result) => !result.Succeeded;
}