using System.Buffers;
using System.Text;
using System.Text.Json;
using TestProjectKeyApp.Models.PlayStoreSuggestionModels.Request;

namespace TestProjectKeyApp.Helpers
{
    /// <summary>
    /// Helper class to build and serialize Google Play RPC search payloads.
    /// Converts models to the expected JSON format for Google Play API requests.
    /// </summary>
    public static class GooglePlayPayloadBuilder
    {
        private const string RPCMethod = "teXCtc";
        private const string Type = "generic";

        //Some magic numbers that Google Play uses for filtering
        private const int FirstFilter = 2;
        private const int SecondFilter = 1;

        /// <summary>
        /// Builds a complete RPC payload JSON string for Google Play search.
        /// </summary>
        /// <param name="searchTerm">The search query term</param>
        /// <param name="resultLimit">Maximum number of results (default 10)</param>
        /// <returns>JSON-encoded RPC payload ready for f.req parameter</returns>
        public static string BuildSearchPayload(string searchTerm, int resultLimit = 10)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(searchTerm, nameof(searchTerm));
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(resultLimit, nameof(resultLimit));

            // Build inner search parameters: [null,[\"search_term\"],[limit],[2,1],4]
            var searchParams = new GooglePlaySearchParams
            {
                Query = new List<string> { searchTerm },
                Limit = new List<int> { resultLimit },
                Filters = new List<int> { FirstFilter, SecondFilter }
            };

            // Serialize search params to JSON
            string searchParamsJson = SerializeSearchParams(searchParams);

            // Build RPC payload: [[[\"teXCtc\",\"<inner_json>\",null,\"generic\"]]]
            var rpcPayload = new List<List<List<string?>>>
            {
                new()
                {
                    new ()
                    {
                        RPCMethod,
                        searchParamsJson,
                        null,
                        Type
                    }
                }
            };

            return JsonSerializer.Serialize(rpcPayload, new JsonSerializerOptions { WriteIndented = false });
        }

        /// <summary>
        /// Serializes GooglePlaySearchParams to the expected compact JSON format.
        /// Output format: [null,["term"],[limit],[2,1],4]
        /// Writes directly via Utf8JsonWriter instead of boxing values into a
        /// List avoiding per-element runtime type resolution and boxing of value types.
        /// </summary>
        private static string SerializeSearchParams(GooglePlaySearchParams @params)
        {
            var bufferWriter = new ArrayBufferWriter<byte>(256);
            using (var writer = new Utf8JsonWriter(bufferWriter))
            {
                writer.WriteStartArray();

                if (@params.Reserved is null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    writer.WriteStringValue(@params.Reserved);
                }

                WriteStringArray(writer, @params.Query);
                WriteIntArray(writer, @params.Limit);
                WriteIntArray(writer, @params.Filters);

                writer.WriteNumberValue(@params.Flags);

                writer.WriteEndArray();
            }

            return Encoding.UTF8.GetString(bufferWriter.WrittenSpan);
        }

        private static void WriteStringArray(Utf8JsonWriter writer, List<string> values)
        {
            writer.WriteStartArray();
            foreach (string value in values)
            {
                writer.WriteStringValue(value);
            }
            writer.WriteEndArray();
        }

        private static void WriteIntArray(Utf8JsonWriter writer, List<int> values)
        {
            writer.WriteStartArray();
            foreach (int value in values)
            {
                writer.WriteNumberValue(value);
            }
            writer.WriteEndArray();
        }
    }
}