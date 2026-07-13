using TestProjectKeyApp.Models;

namespace TestProjectKeyApp.Settings.ISettings;

public interface IAppSettingsProvider
{
    AppSettingsModel LoadAppSettings();
}
