using Microsoft.Extensions.Configuration;
using TestProjectKeyApp.Constants;
using TestProjectKeyApp.Models;
using TestProjectKeyApp.Settings.ISettings;

namespace TestProjectKeyApp.Settings;

public sealed class AppSettingsProvider : IAppSettingsProvider
{
    private readonly IConfiguration _configuration;

    public AppSettingsProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AppSettingsModel LoadAppSettings()
    {
        return _configuration.GetSection(AppConstants.SettingsConfigurationSection).Get<AppSettingsModel>() ?? new AppSettingsModel();
    }
}
