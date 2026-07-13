using Microsoft.Extensions.Configuration;

namespace TestProjectKeyApp.Providers.IProviders;

public interface IApplicationConfigurationBuilder
{
    IConfiguration Build();
}
