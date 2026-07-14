using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestProjectKeyApp.Helpers;
using TestProjectKeyApp.Helpers.IHelpers;
using TestProjectKeyApp.Providers;
using TestProjectKeyApp.Providers.IProviders;
using TestProjectKeyApp.Services;
using TestProjectKeyApp.Services.IServices;
using TestProjectKeyApp.Settings;
using TestProjectKeyApp.Settings.ISettings;

namespace TestProjectKeyApp;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            using (var cts = new CancellationTokenSource())
            {
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

                await using (var serviceProvider = services.BuildServiceProvider())
                {
                    var searchLoopProvider = serviceProvider.GetRequiredService<SearchLoopProvider>();
                    await searchLoopProvider.RunAsync(cts.Token);
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Fatal error: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        var configBuilder = new ApplicationConfigurationBuilder();
        IConfiguration configuration = configBuilder.Build();
        services.AddSingleton(configuration);

        services.AddSingleton<IAppSettingsProvider, AppSettingsProvider>();

        services.AddHttpClient<IAppSearchService, AppSearchService>((serviceProvider, client) =>
        {
            var settings = serviceProvider.GetRequiredService<IAppSettingsProvider>().LoadAppSettings();
            client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
        });

        services.AddSingleton<IOutputProvider, ConsoleOutputWriter>();
        services.AddSingleton<IUserInputProvider, ConsoleUserInputProvider>();
        services.AddSingleton<IAppSearchResponseParser, PlayStoreSuggestionResponseParser>();
        services.AddSingleton<SearchLoopProvider>();
    }
}