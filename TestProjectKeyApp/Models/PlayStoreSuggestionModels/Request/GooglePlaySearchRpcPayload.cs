namespace TestProjectKeyApp.Models.PlayStoreSuggestionModels.Request
{
    /// <summary>
    /// Represents the RPC batch payload for Google Play search requests.
    /// This is serialized to JSON and embedded in the f.req form parameter.
    /// Example structure: [[[\"teXCtc\",\"[null,[\\\"pol\\\"],[10],[2,1],4]\",null,\"generic\"]]]
    /// </summary>
    public class GooglePlaySearchRpcPayload
    {
        /// <summary>
        /// The RPC batch containing the search request.
        /// Typically a single-element array containing the RPC request.
        /// </summary>
        private List<List<List<string?>>> Batch { get; set; } = new();

        /// <summary>
        /// Creates a new RPC payload with a single search request.
        /// </summary>
        /// <param name="rpcMethod">RPC method name (e.g., "teXCtc")</param>
        /// <param name="searchParams">JSON-encoded search parameters</param>
        /// <param name="requestId">Optional request ID</param>
        /// <returns>Configured RPC payload ready for serialization</returns>
        public static GooglePlaySearchRpcPayload CreateSearchRequest(string rpcMethod, string searchParams, string? requestId = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(rpcMethod, nameof(rpcMethod));
            ArgumentException.ThrowIfNullOrWhiteSpace(searchParams, nameof(searchParams));

            var payload = new GooglePlaySearchRpcPayload();
            var inner = new List<string?> { rpcMethod, searchParams, null, requestId ?? "generic" };
            payload.Batch.Add(new List<List<string?>> { new List<string?>(inner) });
            return payload;
        }
    }
}
