namespace TestProjectKeyApp.Models.PlayStoreSuggestionModels.Request
{
    /// <summary>
    /// Represents the search parameters for Google Play RPC request.
    /// This object is JSON-encoded and passed as a string in the RPC payload.
    /// Example: [null,[\"search_term\"],[limit],[2,1],4]
    /// </summary>
    public class GooglePlaySearchParams
    {
        /// <summary>
        /// Reserved/null field
        /// </summary>
        public string? Reserved { get; set; } = null;

        /// <summary>
        /// Search query terms
        /// </summary>
        public List<string> Query { get; set; } = new();

        /// <summary>
        /// Limit parameters (e.g., result limit)
        /// </summary>
        public List<int> Limit { get; set; } = new();

        /// <summary>
        /// Filter/sort parameters
        /// </summary>
        public List<int> Filters { get; set; } = new();

        /// <summary>
        /// Unknown parameter (typically 4)
        /// </summary>
        public int Flags { get; set; } = 4;
    }
}
