using System.Text.Json.Serialization;

namespace TestProjectKeyApp.Models.PlayStoreSuggestionModels.Request
{
    [JsonSerializable(typeof(GooglePlaySearchQueryParams))]
    [JsonSerializable(typeof(Dictionary<string, string>))]
    public partial class GooglePlayQueryParamsJsonContext : JsonSerializerContext;
}
