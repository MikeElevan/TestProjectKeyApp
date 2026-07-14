namespace TestProjectKeyApp.Providers.IProviders;

public interface ISearchLoopProvider
{
    Task RunAsync(CancellationToken cancellationToken = default);
}
