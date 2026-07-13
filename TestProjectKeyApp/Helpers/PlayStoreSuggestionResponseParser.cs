using System.Text.Json;
using TestProjectKeyApp.Constants;
using TestProjectKeyApp.Converters;
using TestProjectKeyApp.Helpers.IHelpers;
using TestProjectKeyApp.Models.PlayStoreSuggestionModels.Response;

namespace TestProjectKeyApp.Helpers
{
    public class PlayStoreSuggestionResponseParser : IAppSearchResponseParser
    {
        private const string AntiXSSPrefix = ")]}'";

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
                    Console.WriteLine(string.Format(AppConstants.DeserializationSuccessMessage, responseData.Suggestions.Count));

                    return responseData.Suggestions.Select(s => s.Text).ToList();
                }
                else
                {
                    Console.WriteLine(AppConstants.DeserializationNullMessage);

                    return Array.Empty<string>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(AppConstants.DeserializationErrorMessage, ex.Message));
                return Array.Empty<string>();
            }


        }
    }
}
