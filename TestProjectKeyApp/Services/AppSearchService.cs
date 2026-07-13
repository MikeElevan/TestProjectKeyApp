using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using TestProjectKeyApp.Constants;
using TestProjectKeyApp.Helpers;
using TestProjectKeyApp.Helpers.IHelpers;
using TestProjectKeyApp.Models;
using TestProjectKeyApp.Models.PlayStoreSuggestionModels.Request;
using TestProjectKeyApp.Providers.IProviders;
using TestProjectKeyApp.Services.IServices;
using TestProjectKeyApp.Settings.ISettings;

namespace TestProjectKeyApp.Services;

public sealed class AppSearchService : IAppSearchService
{
    private readonly HttpClient _httpClient;
    private readonly IAppSettingsProvider _settingsProvider;
    private readonly IAppSearchResponseParser _responseParser;
    private readonly IOutputProvider _outputProvider;


    public AppSearchService(HttpClient httpClient, IAppSettingsProvider settingsProvider, IAppSearchResponseParser responseParser, IOutputProvider outputProvider)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
        _responseParser = responseParser ?? throw new ArgumentNullException(nameof(responseParser));
        _outputProvider = outputProvider ?? throw new ArgumentNullException(nameof(outputProvider));
    }

    public async Task<IReadOnlyList<string>> SearchAsync(string keyword, string country)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyword, nameof(keyword));
        ArgumentException.ThrowIfNullOrWhiteSpace(country, nameof(country));

        AppSettingsModel settings = _settingsProvider.LoadAppSettings();
        GooglePlaySearchRequest req = GooglePlaySearchRequestBuilder.Build(settings, keyword);
        (Uri requestUri, Dictionary<string, string> bodyDict) = await BuildRequestUri(settings, req, country);

        for (int attempt = 1; attempt <= settings.MaxRetries; attempt++)
        {
            try
            {
                using (var content = new FormUrlEncodedContent(bodyDict))
                using (HttpResponseMessage response = await _httpClient.PostAsync(requestUri, content))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        _outputProvider.WriteErrorLine(string.Format(AppConstants.RequestFailedMessage, (int)response.StatusCode, response.StatusCode, attempt, settings.MaxRetries));
                        if (attempt == settings.MaxRetries)
                        {
                            throw new HttpRequestException(string.Format(AppConstants.SearchRequestFailedMessage, (int)response.StatusCode));
                        }

                        await Task.Delay(settings.RetryDelayMilliseconds);
                        continue;
                    }

                    string responseContent = await response.Content.ReadAsStringAsync();
                    return _responseParser.Parse(responseContent);
                }
            }
            catch (HttpRequestException ex)
            {
                _outputProvider.WriteErrorLine(string.Format(AppConstants.HttpErrorMessage, ex.Message, attempt, settings.MaxRetries));
                if (attempt == settings.MaxRetries)
                {
                    throw;
                }

                await Task.Delay(settings.RetryDelayMilliseconds);
            }
            catch (Exception ex)
            {
                _outputProvider.WriteErrorLine(string.Format(AppConstants.ErrorMessage, ex.Message, attempt, settings.MaxRetries));
                return Array.Empty<string>();
            }
        }

        return Array.Empty<string>();
    }

    private async Task<(Uri, Dictionary<string, string>)> BuildRequestUri(AppSettingsModel settings, GooglePlaySearchRequest req, string country)
    {
        Dictionary<string, string> queryDict = GetDictionaryFromModel(req.Query,
            GooglePlayQueryParamsJsonContext.Default.DictionaryStringString,
            GooglePlayQueryParamsJsonContext.Default.GooglePlaySearchQueryParams);

        using (var content = new FormUrlEncodedContent(queryDict))
        {
            string queryString = await content.ReadAsStringAsync();

            Dictionary<string, string> bodyDict = GetDictionaryFromModel(req.Body,
                GooglePlayBodyParamsJsonContext.Default.DictionaryStringString,
                GooglePlayBodyParamsJsonContext.Default.GooglePlaySearchBodyParams);

            var uriBuilder = new UriBuilder(settings.BaseUrl);
            uriBuilder.Query = queryString;

            return (uriBuilder.Uri, bodyDict);
        }
    }

    private Dictionary<string, string> GetDictionaryFromModel<T>(T model, JsonTypeInfo<Dictionary<string, string>> dict, JsonTypeInfo<T> jsonTypeInfo)
    {
        string jsonQuery = JsonSerializer.Serialize(model, jsonTypeInfo);

        try
        {
            var stringDict = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonQuery, dict);
            return stringDict ?? new Dictionary<string, string>();
        }
        catch (Exception ex)
        {
            _outputProvider.WriteErrorLine(string.Format(AppConstants.ErrorPrefix, ex.Message));
            return new Dictionary<string, string>();
        }
    }
}
