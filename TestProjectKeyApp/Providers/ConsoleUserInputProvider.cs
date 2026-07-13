using TestProjectKeyApp.Providers.IProviders;

namespace TestProjectKeyApp.Providers;

public class ConsoleUserInputProvider : IUserInputProvider
{
    public string ReadLine(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? string.Empty;
    }
}
