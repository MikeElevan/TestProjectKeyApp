namespace TestProjectKeyApp.Models.PlayStoreSuggestionModels.Request
{
    public class GooglePlaySearchRequest
    {
        public GooglePlaySearchQueryParams Query { get; set; } = new();
        public GooglePlaySearchBodyParams Body { get; set; } = new();
    }
}
