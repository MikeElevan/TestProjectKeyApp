using TestProjectKeyApp.Providers.IProviders;

namespace TestProjectKeyApp.Providers;

public class ConsoleOutputWriter : IOutputProvider
{
    public void WriteLine(string message)
    {
        Console.WriteLine(message);
    }

    public void WriteErrorLine(string message)
    {
        Console.Error.WriteLine(message);
    }
}
