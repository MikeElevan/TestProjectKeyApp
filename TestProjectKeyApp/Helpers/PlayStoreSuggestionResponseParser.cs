using System.Text.Json;
using TestProjectKeyApp.Constants;
using TestProjectKeyApp.Converters;
using TestProjectKeyApp.Helpers.IHelpers;
using TestProjectKeyApp.Models.PlayStoreSuggestionModels.Response;
using TestProjectKeyApp.Providers.IProviders;

namespace TestProjectKeyApp.Helpers
{
    public class PlayStoreSuggestionResponseParser : IAppSearchResponseParser
    {
        private const string AntiXSSPrefix = ")]}'";

        private readonly IOutputProvider _outputProvider;

        public PlayStoreSuggestionResponseParser(IOutputProvider outputProvider)
        {
            _outputProvider = outputProvider ?? throw new ArgumentNullException(nameof(outputProvider));
        }

        public IReadOnlyList<string> Parse(string responseContent)
        {
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                return Array.Empty<string>();
            }

            if (responseContent.StartsWith(AntiXSSPrefix))
            {
                int startIndex = AntiXSSPrefix.Length;
                responseContent = responseContent[startIndex..].Trim();
            }

            var options = new JsonSerializerOptions();
            options.Converters.Add(new PlayStoreSuggestionConverter());

            try
            {
                var responseData = JsonSerializer.Deserialize<PlayStoreSuggestionResponseModel>(responseContent, options);

                if (responseData != null)
                {
                    _outputProvider.WriteLine(string.Format(AppConstants.DeserializationSuccessMessage, responseData.Suggestions.Count));

                    return responseData.Suggestions.Select(s => s.Text).ToList();
                }
                else
                {
                    _outputProvider.WriteLine(AppConstants.DeserializationNullMessage);

                    return Array.Empty<string>();
                }
            }
            catch (Exception ex)
            {
                _outputProvider.WriteErrorLine(string.Format(AppConstants.DeserializationErrorMessage, ex.Message));
                return Array.Empty<string>();
            }


        }
    }
}
