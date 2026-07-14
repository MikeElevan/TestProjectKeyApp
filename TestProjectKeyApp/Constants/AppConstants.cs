namespace TestProjectKeyApp.Constants;

public static class AppConstants
{
    public const string AppSettingsFileName = "appsettings.json";

    public const string EnterKeywordPrompt = "Enter keyword (or 'exit' to quit): ";
    public const string EnterCountryPrompt = "Enter country: ";
    public const string EmptyKeywordOrCountryMessage = "Keyword and country must not be empty.";
    public const string ExitMessage = "Goodbye!";
    public const string ErrorPrefix = "Error: ";
    public const string ErrorMessage = "Error: {0}. Attempt {1}/{2}.";
    public const string SettingsConfigurationSection = "Settings";

    /// <summary>
    /// Request failed message with status
    /// Parameters: {0} - HTTP status code (int), {1} - HTTP status (enum), {2} - current attempt, {3} - max attempts
    /// </summary>
    public const string RequestFailedMessage = "Request failed with status {0} ({1}). Attempt {2}/{3}.";

    /// <summary>
    /// Search request failed message with status code
    /// Parameters: {0} - HTTP status code (int)
    /// </summary>
    public const string SearchRequestFailedMessage = "Search request failed with status code {0}.";

    /// <summary>
    /// Request timeout message
    /// Parameters: {0} - current attempt, {1} - max attempts
    /// </summary>
    public const string RequestTimeoutMessage = "The request timed out. Attempt {0}/{1}.";
    public const string RequestTimeoutException = "The request timed out.";

    /// <summary>
    /// HTTP error message
    /// Parameters: {0} - error message, {1} - current attempt, {2} - max attempts
    /// </summary>
    public const string HttpErrorMessage = "HTTP error: {0}. Attempt {1}/{2}.";

    // PlayStoreSuggestionResponseParser messages

    /// <summary>
    /// Deserialization success message
    /// Parameters: {0} - suggestion count
    /// </summary>
    public const string DeserializationSuccessMessage = "Deserialization successful. Suggestion count: {0}";
    public const string DeserializationNullMessage = "Deserialization returned null.";

    /// <summary>
    /// Deserialization error message
    /// Parameters: {0} - error message
    /// </summary>
    public const string DeserializationErrorMessage = "Deserialization error: {0}";
    public const string ConverterMessage = "Serialization back to Google's format is not implemented.";
}
