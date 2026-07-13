using System.Text.Json;
using System.Text.Json.Serialization;
using TestProjectKeyApp.Constants;
using TestProjectKeyApp.Models.PlayStoreSuggestionModels.Response;

namespace TestProjectKeyApp.Converters
{
    public class PlayStoreSuggestionConverter : JsonConverter<PlayStoreSuggestionResponseModel>
    {
        
        private const int HintTextPosition = 0;
        private const int ResponeDataPosition = 0;
        private const int ArrayWithHintsPosition = 2;
        private const int HintsPosition = 0;
        private const int HintSearchUrlPosition = 2;
        private const int HintUrlArrayPosition = 2;
        private const int HintUrlDetailsPosition = 4;
        private const int HintUrlArrayExcpetedSize = 4;
        private const int HintUrlDetailsExcpetedSize = 2;
        private const int HintsArrayExcpetedSize = 3;

        public override PlayStoreSuggestionResponseModel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = new PlayStoreSuggestionResponseModel();

            // The input JSON is a top-level array
            using (var doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;

                if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
                {
                    return result;
                }

                // The first element of the array contains the response data (index 0)
                var batchResponse = root[ResponeDataPosition];
                if (batchResponse.ValueKind != JsonValueKind.Array || batchResponse.GetArrayLength() < 3)
                {
                    return result;
                }

                // Google packages inner data as a JSON-encoded string at index 2
                var innerJsonString = batchResponse[ArrayWithHintsPosition].GetString();
                if (string.IsNullOrEmpty(innerJsonString))
                {
                    return result;
                }

                // Parse the unpacked inner JSON string (again an array of arrays)
                using (var innerDoc = JsonDocument.Parse(innerJsonString))
                {
                    var innerRoot = innerDoc.RootElement;

                    if (innerRoot.ValueKind != JsonValueKind.Array || innerRoot.GetArrayLength() == 0)
                    {
                        return result;
                    }

                    // Suggestions are located in the first top-level array of the inner document
                    var suggestionsArray = innerRoot[HintsPosition];
                    if (suggestionsArray.ValueKind != JsonValueKind.Array)
                    {
                        return result;
                    }

                    foreach (var item in suggestionsArray.EnumerateArray())
                    {
                        if (item.ValueKind != JsonValueKind.Array || item.GetArrayLength() < HintsArrayExcpetedSize)
                        {
                            continue;
                        }

                        // Index 0: suggestion text ("polybuzz")
                        var text = item[HintTextPosition].GetString() ?? string.Empty;

                        // Index 2: an array with URLs. Inside it at index 4 there's another array,
                        // where at position 2 there is the relative search link.
                        string url = string.Empty;
                        var detailsContainer = item[HintUrlArrayPosition];

                        if (detailsContainer.ValueKind == JsonValueKind.Array && detailsContainer.GetArrayLength() > HintUrlArrayExcpetedSize)
                        {
                            var urlContainer = detailsContainer[HintUrlDetailsPosition];
                            if (urlContainer.ValueKind == JsonValueKind.Array && urlContainer.GetArrayLength() > HintUrlDetailsExcpetedSize)
                            {
                                url = urlContainer[HintSearchUrlPosition].GetString() ?? string.Empty;
                            }
                        }

                        result.Suggestions.Add(new SuggestionItemModel
                        {
                            Text = text,
                            SearchUrl = url
                        });
                    }
                }

            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, PlayStoreSuggestionResponseModel value, JsonSerializerOptions options)
        {
            throw new NotImplementedException(AppConstants.ConverterMessage);
        }
    }
}
