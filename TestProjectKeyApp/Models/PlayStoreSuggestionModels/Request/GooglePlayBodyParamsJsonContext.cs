using System.Text.Json.Serialization;

namespace TestProjectKeyApp.Models.PlayStoreSuggestionModels.Request
{
    [JsonSerializable(typeof(GooglePlaySearchBodyParams))]
    [JsonSerializable(typeof(Dictionary<string, string>))]
    public partial class GooglePlayBodyParamsJsonContext : JsonSerializerContext;
}
