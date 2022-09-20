using System.Text.Json;
using System.Text.Json.Serialization;

namespace JwtSample.Services.JsonService;

#nullable disable
public class JsonService : ISerializerService
{
    public T Deserialize<T>(string text)
    {
        return JsonSerializer.Deserialize<T>(text);
    }

    public string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        });
    }

    public string Serialize<T>(T obj, Type type)
    {
        return JsonSerializer.Serialize(obj, type);
    }
}
