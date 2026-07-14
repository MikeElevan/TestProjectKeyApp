using TestProjectKeyApp.Models;
using TestProjectKeyApp.Models.PlayStoreSuggestionModels.Request;

namespace TestProjectKeyApp.Helpers
{
    public static class GooglePlaySearchRequestBuilder
    {
        private const int MaxResultNumber = 10;

        /// <summary>
        /// Builds a complete GooglePlaySearchRequest with parameters from RequestParams section in appsettings.json.
        /// </summary>
        /// <param name="searchTerm">The search query term</param>
        /// <param name="resultLimit">Maximum number of results (default 10)</param>
        /// <returns>Fully configured GooglePlaySearchRequest ready to send to Google Play API</returns>
        public static GooglePlaySearchRequest Build(AppSettingsModel appSettingsModel, string searchTerm, int resultLimit = MaxResultNumber)
        {
            ArgumentNullException.ThrowIfNull(appSettingsModel, nameof(appSettingsModel));
            ArgumentException.ThrowIfNullOrWhiteSpace(searchTerm, nameof(searchTerm));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(resultLimit, nameof(resultLimit));

            GooglePlaySearchQueryParams queryParams = appSettingsModel.RequestParams ?? new GooglePlaySearchQueryParams();

            var bodyParams = new GooglePlaySearchBodyParams
            {
                FReq = GooglePlayPayloadBuilder.BuildSearchPayload(searchTerm, resultLimit),
                At = queryParams.At
            };

            return new GooglePlaySearchRequest
            {
                Query = queryParams,
                Body = bodyParams
            };
        }
    }
}
