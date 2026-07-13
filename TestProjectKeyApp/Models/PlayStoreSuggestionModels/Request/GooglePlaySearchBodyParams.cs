using System.Text.Json.Serialization;

namespace TestProjectKeyApp.Models.PlayStoreSuggestionModels.Request
{
    /// <summary>
    /// application/x-www-form-urlencoded body parameters.
    /// </summary>
    public class GooglePlaySearchBodyParams
    {
        /// <summary>
        /// JSON-encoded RPC batch payload.
        /// Example: [[[\"teXCtc\",\"[null,[\\\"pol\\\"],[10],[2,1],4]\",null,\"generic\"]]]
        /// </summary>
        [JsonPropertyName("f.req")]
        public string FReq { get; set; } = string.Empty;

        /// <summary>
        /// Anti-CSRF / session auth token.
        /// Example: "AMLJbAnRcbYMQEOed2IPDgTjoTg3:1783718899927"
        /// </summary>
        [JsonPropertyName("at")]
        public string At { get; set; } = string.Empty;
    }
}