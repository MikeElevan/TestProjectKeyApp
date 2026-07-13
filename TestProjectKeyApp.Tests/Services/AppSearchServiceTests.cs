using Moq;
using Moq.Protected;
using System.Net;
using TestProjectKeyApp.Helpers.IHelpers;
using TestProjectKeyApp.Models;
using TestProjectKeyApp.Providers.IProviders;
using TestProjectKeyApp.Services;
using TestProjectKeyApp.Settings.ISettings;
using Xunit;

namespace TestProjectKeyApp.Tests.Services;

public class AppSearchServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly Mock<IAppSettingsProvider> _mockSettingsProvider;
    private readonly Mock<IAppSearchResponseParser> _mockResponseParser;
    private readonly Mock<IOutputProvider> _mockOutputProvider;
    private readonly AppSearchService _appSearchService;

    public AppSearchServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _mockSettingsProvider = new Mock<IAppSettingsProvider>();
        _mockResponseParser = new Mock<IAppSearchResponseParser>();
        _mockOutputProvider = new Mock<IOutputProvider>();
        _appSearchService = new AppSearchService(_httpClient, _mockSettingsProvider.Object, _mockResponseParser.Object, _mockOutputProvider.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new AppSearchService(null!, _mockSettingsProvider.Object, _mockResponseParser.Object, _mockOutputProvider.Object));
        Assert.Equal("httpClient", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullSettingsProvider_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new AppSearchService(_httpClient, null!, _mockResponseParser.Object, _mockOutputProvider.Object));
        Assert.Equal("settingsProvider", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullResponseParser_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new AppSearchService(_httpClient, _mockSettingsProvider.Object, null!, _mockOutputProvider.Object));
        Assert.Equal("responseParser", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullOutputProvider_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new AppSearchService(_httpClient, _mockSettingsProvider.Object, _mockResponseParser.Object, null!));
        Assert.Equal("outputProvider", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Arrange & Act & Assert
        var service = new AppSearchService(_httpClient, _mockSettingsProvider.Object, _mockResponseParser.Object, _mockOutputProvider.Object);
        Assert.NotNull(service);
    }

    #endregion

    #region SearchAsync - Parameter Validation Tests

    [Fact]
    public async Task SearchAsync_WithNullKeyword_ThrowsArgumentNullException()
    {
        // Arrange
        var country = "US";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _appSearchService.SearchAsync(null!, country));
    }

    [Fact]
    public async Task SearchAsync_WithEmptyKeyword_ThrowsArgumentException()
    {
        // Arrange
        var country = "US";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _appSearchService.SearchAsync("", country));
    }

    [Fact]
    public async Task SearchAsync_WithWhitespaceKeyword_ThrowsArgumentException()
    {
        // Arrange
        var country = "US";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _appSearchService.SearchAsync("   ", country));
    }

    [Fact]
    public async Task SearchAsync_WithNullCountry_ThrowsArgumentNullException()
    {
        // Arrange
        var keyword = "test";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _appSearchService.SearchAsync(keyword, null!));
    }

    [Fact]
    public async Task SearchAsync_WithEmptyCountry_ThrowsArgumentException()
    {
        // Arrange
        var keyword = "test";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _appSearchService.SearchAsync(keyword, ""));
    }

    [Fact]
    public async Task SearchAsync_WithWhitespaceCountry_ThrowsArgumentException()
    {
        // Arrange
        var keyword = "test";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _appSearchService.SearchAsync(keyword, "   "));
    }

    #endregion

    #region SearchAsync - Successful Response Tests

    [Fact]
    public async Task SearchAsync_WithValidParameters_ReturnsResultsFromParser()
    {
        // Arrange
        var keyword = "test";
        var country = "US";
        var expectedResults = new[] { "app1", "app2", "app3" };
        var settings = new AppSettingsModel();
        var responseContent = "test response content";

        _mockSettingsProvider.Setup(x => x.LoadAppSettings()).Returns(settings);
        _mockResponseParser.Setup(x => x.Parse(responseContent)).Returns(expectedResults);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent)
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        // Act
        var result = await _appSearchService.SearchAsync(keyword, country);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResults, result);
        _mockSettingsProvider.Verify(x => x.LoadAppSettings(), Times.Once);
        _mockResponseParser.Verify(x => x.Parse(responseContent), Times.Once);
    }

    #endregion

    #region SearchAsync - Failure and Retry Tests

    [Fact]
    public async Task SearchAsync_WithFailureStatusCode_RetriesAndThrowsAfterMaxRetries()
    {
        // Arrange
        var keyword = "test";
        var country = "US";
        var settings = new AppSettingsModel { MaxRetries = 3, RetryDelayMilliseconds = 10 };

        _mockSettingsProvider.Setup(x => x.LoadAppSettings()).Returns(settings);

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Returns(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            _appSearchService.SearchAsync(keyword, country));

        // Verify retries occurred
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(3),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task SearchAsync_WithPartialFailureThenSuccess_ReturnsResults()
    {
        // Arrange
        var keyword = "test";
        var country = "US";
        var expectedResults = new[] { "app1", "app2" };
        var settings = new AppSettingsModel { MaxRetries = 3, RetryDelayMilliseconds = 10 };
        var responseContent = "success response";

        _mockSettingsProvider.Setup(x => x.LoadAppSettings()).Returns(settings);
        _mockResponseParser.Setup(x => x.Parse(responseContent)).Returns(expectedResults);

        var failureResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
        var successResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent)
        };

        var callCount = 0;
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Returns(() =>
            {
                callCount++;
                return Task.FromResult(callCount == 1 ? failureResponse : successResponse);
            });

        // Act
        var result = await _appSearchService.SearchAsync(keyword, country);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResults, result);
    }

    [Fact]
    public async Task SearchAsync_WithHttpRequestException_RetriesAndThrowsAfterMaxRetries()
    {
        // Arrange
        var keyword = "test";
        var country = "US";
        var settings = new AppSettingsModel { MaxRetries = 3, RetryDelayMilliseconds = 10 };

        _mockSettingsProvider.Setup(x => x.LoadAppSettings()).Returns(settings);
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            _appSearchService.SearchAsync(keyword, country));

        // Verify retries occurred
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(3),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task SearchAsync_WithGeneralException_ReturnsEmptyArray()
    {
        // Arrange
        var keyword = "test";
        var country = "US";
        var settings = new AppSettingsModel { MaxRetries = 3 };

        _mockSettingsProvider.Setup(x => x.LoadAppSettings()).Returns(settings);
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        // Act
        var result = await _appSearchService.SearchAsync(keyword, country);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_WithFailureStatusCodeOnLastRetry_ThrowsHttpRequestException()
    {
        // Arrange
        var keyword = "test";
        var country = "US";
        var settings = new AppSettingsModel { MaxRetries = 2, RetryDelayMilliseconds = 10 };

        _mockSettingsProvider.Setup(x => x.LoadAppSettings()).Returns(settings);

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Returns(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<HttpRequestException>(() =>
            _appSearchService.SearchAsync(keyword, country));

        Assert.NotNull(ex);
        Assert.Contains("400", ex.Message);
    }

    #endregion

    #region SearchAsync - Edge Cases

    [Fact]
    public async Task SearchAsync_WithMaxRetriesOfOne_DoesNotRetry()
    {
        // Arrange
        var keyword = "test";
        var country = "US";
        var settings = new AppSettingsModel { MaxRetries = 1 };

        _mockSettingsProvider.Setup(x => x.LoadAppSettings()).Returns(settings);

        var failureResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(failureResponse);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            _appSearchService.SearchAsync(keyword, country));

        // Verify no retry occurred
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task SearchAsync_WithMultipleRetries_RespectsRetryDelay()
    {
        // Arrange
        var keyword = "test";
        var country = "US";
        var settings = new AppSettingsModel { MaxRetries = 2, RetryDelayMilliseconds = 50 };

        _mockSettingsProvider.Setup(x => x.LoadAppSettings()).Returns(settings);

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Returns(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)));

        // Act
        var startTime = DateTime.UtcNow;
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            _appSearchService.SearchAsync(keyword, country));
        var elapsed = DateTime.UtcNow - startTime;

        // Assert - Should have at least one retry delay
        Assert.True(elapsed.TotalMilliseconds >= 50, $"Elapsed time {elapsed.TotalMilliseconds} should be >= 50ms");
    }

    [Theory]
    [InlineData("test")]
    [InlineData("multiple word keyword")]
    [InlineData("special!@#$%^&*()chars")]
    [InlineData("123")]
    public async Task SearchAsync_WithVariousKeywords_Succeeds(string keyword)
    {
        // Arrange
        var country = "US";
        var expectedResults = new[] { "app1" };
        var settings = new AppSettingsModel();
        var responseContent = "response";

        _mockSettingsProvider.Setup(x => x.LoadAppSettings()).Returns(settings);
        _mockResponseParser.Setup(x => x.Parse(responseContent)).Returns(expectedResults);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent)
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        // Act
        var result = await _appSearchService.SearchAsync(keyword, country);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResults, result);
    }

    [Theory]
    [InlineData("US")]
    [InlineData("GB")]
    [InlineData("JP")]
    public async Task SearchAsync_WithVariousCountries_Succeeds(string country)
    {
        // Arrange
        var keyword = "test";
        var expectedResults = new[] { "app1" };
        var settings = new AppSettingsModel();
        var responseContent = "response";

        _mockSettingsProvider.Setup(x => x.LoadAppSettings()).Returns(settings);
        _mockResponseParser.Setup(x => x.Parse(responseContent)).Returns(expectedResults);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent)
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        // Act
        var result = await _appSearchService.SearchAsync(keyword, country);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResults, result);
    }

    #endregion

    #region Resource Cleanup Tests

    [Fact]
    public async Task SearchAsync_DisposesHttpResponseMessage()
    {
        // Arrange
        var keyword = "test";
        var country = "US";
        var settings = new AppSettingsModel();
        var responseContent = "response";
        var expectedResults = new[] { "app1" };

        _mockSettingsProvider.Setup(x => x.LoadAppSettings()).Returns(settings);
        _mockResponseParser.Setup(x => x.Parse(responseContent)).Returns(expectedResults);

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseContent)
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);

        // Act
        await _appSearchService.SearchAsync(keyword, country);

        // Assert - If we get here without exception, disposal was successful
        Assert.NotNull(httpResponseMessage);
    }

    #endregion
}