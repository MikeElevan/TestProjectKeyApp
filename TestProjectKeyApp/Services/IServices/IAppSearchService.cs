namespace TestProjectKeyApp.Services.IServices;

public interface IAppSearchService
{
    Task<IReadOnlyList<string>> SearchAsync(string keyword, string country);
}
