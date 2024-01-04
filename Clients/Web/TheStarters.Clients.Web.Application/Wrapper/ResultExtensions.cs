using System.Text.Json;
using System.Text.Json.Serialization;

namespace TheStarters.Clients.Web.Application.Wrapper;

public static class ResultExtensions
{
    private readonly static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.Preserve
    };
    public static async Task<IResult<T>> ToResultAsync<T>(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        try
        {
            var responseObject = JsonSerializer.Deserialize<Result<T>>(responseAsString, _serializerOptions);
            if (!response.IsSuccessStatusCode && !responseObject!.Messages.Any())
            {
                var apiControllerResponse = JsonSerializer.Deserialize<ApiControllerError>(responseAsString, _serializerOptions);
                if (apiControllerResponse != null)
                    return Result<T>.Fail(apiControllerResponse.Title);
            }
            return responseObject ?? Result<T>.Fail("Fail when deserialize response");
        }
        catch (Exception ex)
        {
            return await Result<T>.FailAsync(ex.Message);
        }

    }
    public static async Task<IResult> ToResult(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        try
        {
            var responseObject = JsonSerializer.Deserialize<Result>(responseAsString, _serializerOptions);
            return responseObject;
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }

    }

    public static async Task<PaginatedResult<T>> ToPaginatedResult<T>(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        try
        {
            var responseObject = JsonSerializer.Deserialize<PaginatedResult<T>>(responseAsString, _serializerOptions);
            return responseObject;
        }
        catch (Exception ex)
        {
            return new PaginatedResult<T>(false, messages: new() { ex.Message });
        }

    }


    public class ApiControllerError
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string TraceId { get; set; }
    }
}
