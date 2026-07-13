using Microsoft.Extensions.Configuration;
using TestProjectKeyApp.Constants;
using TestProjectKeyApp.Providers.IProviders;

namespace TestProjectKeyApp.Providers;

public class ApplicationConfigurationBuilder : IApplicationConfigurationBuilder
{
    public IConfiguration Build()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(AppConstants.AppSettingsFileName, optional: false, reloadOnChange: false)
            .Build();
    }
}
