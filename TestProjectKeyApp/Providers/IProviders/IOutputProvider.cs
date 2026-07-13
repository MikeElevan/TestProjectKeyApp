namespace TestProjectKeyApp.Providers.IProviders;

public interface IOutputProvider
{
    void WriteLine(string message);
    void WriteErrorLine(string message);
}
