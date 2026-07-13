namespace TestProjectKeyApp.Models
{
    public class AppSettingsModel
    {
        public int Limit { get; set; } = 20;
        public int TimeoutSeconds { get; set; } = 30;
        public int MaxRetries { get; set; } = 3;
        public int RetryDelayMilliseconds { get; set; } = 1000;
        public string BaseUrl { get; set; } = "https://play.google.com/_/PlayStoreUi/data/batchexecute";
        public RequestParamsModel RequestParams { get; set; } = new RequestParamsModel();
    }
}
