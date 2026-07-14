using System.Text.Json.Serialization;

namespace TestProjectKeyApp.Models.PlayStoreSuggestionModels.Request
{
    public class GooglePlaySearchQueryParams
    {
        [JsonPropertyName("rpcids")]
        public string Rpcids { get; set; } = string.Empty;

        [JsonPropertyName("source-path")]
        public string SourcePath { get; set; } = string.Empty;

        [JsonPropertyName("bl")]
        public string Bl { get; set; } = string.Empty;

        [JsonPropertyName("hl")]
        public string Hl { get; set; } = string.Empty;

        [JsonPropertyName("gl")]
        public string Gl { get; set; } = string.Empty;

        [JsonPropertyName("authuser")]
        public string AuthUser { get; set; } = string.Empty;

        [JsonPropertyName("soc-app")]
        public string SocApp { get; set; } = string.Empty;

        [JsonPropertyName("soc-platform")]
        public string SocPlatform { get; set; } = string.Empty;

        [JsonPropertyName("soc-device")]
        public string SocDevice { get; set; } = string.Empty;

        [JsonPropertyName("_reqid")]
        public string ReqId { get; set; } = string.Empty;

        [JsonPropertyName("rt")]
        public string Rt { get; set; } = string.Empty;

        [JsonPropertyName("f.sid")]
        public string FSid { get; set; } = string.Empty;

        /// <summary>
        /// Anti-CSRF / session auth token, config-only — belongs in the request body, not the querystring.
        /// </summary>
        [JsonIgnore]
        public string At { get; set; } = string.Empty;
    }

}