namespace TestProjectKeyApp.Helpers.IHelpers;

public interface IAppSearchResponseParser
{
    IReadOnlyList<string> Parse(string responseContent);
}
