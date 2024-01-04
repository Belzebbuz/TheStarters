using System.Text.Json;
using System.Text.Json.Serialization;

namespace TheStarters.Clients.Web.Infrastructure.JsonConverters;

public class NullableGuidArrayConverter : JsonConverter<Guid?[,]>
{

	public override Guid?[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}

	public override void Write(Utf8JsonWriter writer, Guid?[,] value, JsonSerializerOptions options)
	{
		var convertedArray = ConvertArray(value);
		JsonSerializer.Serialize(writer, convertedArray, options);
	}
	
	private static Guid?[][] ConvertArray(Guid?[,] source)
	{
		var rows = source.GetLength(0);
		var cols = source.GetLength(1);
		var result = new Guid?[rows][];
		for (var i = 0; i < rows; i++)
		{
			result[i] = new Guid?[cols];
			for (var j = 0; j < cols; j++)
			{
				result[i][j] = source[i, j];
			}
		}
		return result;
	}
}