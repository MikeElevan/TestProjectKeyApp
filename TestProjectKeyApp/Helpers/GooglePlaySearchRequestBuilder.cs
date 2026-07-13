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

            var requestParams = appSettingsModel.RequestParams ?? new RequestParamsModel();

            var queryParams = LoadQueryParams(requestParams);
            var bodyParams = LoadBodyParams(searchTerm, resultLimit, requestParams);

            return new GooglePlaySearchRequest
            {
                Query = queryParams,
                Body = bodyParams
            };
        }

        /// <summary>
        /// Loads query parameters from RequestParams section in appsettings.json.
        /// </summary>
        /// <param name="requestParams">The request parameters model</param>
        /// <returns>Configured query parameters</returns>
        private static GooglePlaySearchQueryParams LoadQueryParams(RequestParamsModel requestParams)
        {
            return new GooglePlaySearchQueryParams
            {
                Rpcids = requestParams.Rpcids ?? string.Empty,
                SourcePath = requestParams.SourcePath ?? string.Empty,
                Bl = requestParams.Bl ?? string.Empty,
                Hl = requestParams.Hl ?? string.Empty,
                Gl = requestParams.Gl ?? string.Empty,
                AuthUser = requestParams.AuthUser ?? string.Empty,
                SocApp = requestParams.SocApp ?? string.Empty,
                SocPlatform = requestParams.SocPlatform ?? string.Empty,
                SocDevice = requestParams.SocDevice ?? string.Empty,
                ReqId = requestParams.ReqId ?? string.Empty,
                Rt = requestParams.Rt ?? string.Empty,
                FSid = requestParams.FSid ?? string.Empty
            };
        }

        /// <summary>
        /// Loads body parameters from RequestParams section and builds RPC payload for the given search term.
        /// </summary>
        /// <param name="requestParams">The request parameters model</param>
        /// <param name="searchTerm">The search query term</param>
        /// <param name="resultLimit">Maximum number of results</param>
        /// <returns>Configured body parameters</returns>
        private static GooglePlaySearchBodyParams LoadBodyParams(string searchTerm, int resultLimit, RequestParamsModel? requestParams)
        {
            requestParams = requestParams ?? new RequestParamsModel();

            // Build RPC payload JSON string
            string fReqPayload = GooglePlayPayloadBuilder.BuildSearchPayload(searchTerm, resultLimit);

            return new GooglePlaySearchBodyParams
            {
                FReq = fReqPayload,
                At = requestParams.At ?? string.Empty
            };
        }
    }


}
