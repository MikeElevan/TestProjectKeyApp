using TestProjectKeyApp.Constants;
using TestProjectKeyApp.Providers.IProviders;
using TestProjectKeyApp.Services.IServices;

namespace TestProjectKeyApp.Providers;

public class SearchLoopProvider : ISearchLoopProvider
{
    private readonly IUserInputProvider _inputProvider;
    private readonly IOutputProvider _outputProvider;
    private readonly IAppSearchService _searchService;

    public SearchLoopProvider(
        IUserInputProvider inputProvider,
        IOutputProvider outputProvider,
        IAppSearchService searchService)
    {
        _inputProvider = inputProvider ?? throw new ArgumentNullException(nameof(inputProvider));
        _outputProvider = outputProvider ?? throw new ArgumentNullException(nameof(outputProvider));
        _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
    }

    public async Task RunAsync()
    {
        while (true)
        {
            string keyword = _inputProvider.ReadLine(AppConstants.EnterKeywordPrompt).Trim();
            string country = _inputProvider.ReadLine(AppConstants.EnterCountryPrompt).Trim();

            if (string.IsNullOrWhiteSpace(keyword) || string.IsNullOrWhiteSpace(country))
            {
                _outputProvider.WriteLine(AppConstants.EmptyKeywordOrCountryMessage);
                continue;
            }

            try
            {
                IReadOnlyList<string> results = await _searchService.SearchAsync(keyword, country);
                _outputProvider.WriteLine(string.Join(Environment.NewLine, results));
            }
            catch (Exception ex)
            {
                _outputProvider.WriteErrorLine(AppConstants.ErrorPrefix + ex.Message);
            }
        }
    }
}
