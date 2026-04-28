using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using PriceSentry.Application.Common.Exceptions;
using PriceSentry.Persistence.Interfases;
using PriceSentry.Persistence.Services.Shops;
using System.Net;
using System.Text;
using Xunit;

namespace PriceSentry.Tests.Services.Shops;

public class CitilinkParserPriceTests {
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<IShopPriceParser>> _loggerMock;
    private readonly CitilinkParserPrice _parser;

    public CitilinkParserPriceTests() {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _loggerMock = new Mock<ILogger<IShopPriceParser>>();
        _parser = new CitilinkParserPrice(_httpClient, _loggerMock.Object);
    }

    #region CanParse Tests

    [Fact]
    public void CanParse_WhenUrlContainsCitilinkRu_ReturnsTrue() {
        // Arrange
        var urls = new[]
        {
            "https://www.citilink.ru/product/123456/",
            "https://citilink.ru/catalog/smartfony/",
            "http://citilink.ru/product/",
            "https://m.citilink.ru/product/test"
        };

        foreach (var url in urls) {
            // Act
            var result = _parser.CanParse(url);

            // Assert
            Assert.True(result, $"URL: {url}");
        }
    }

    [Fact]
    public void CanParse_WhenUrlDoesNotContainCitilinkRu_ReturnsFalse() {
        // Arrange
        var urls = new[]
        {
            "https://google.com",
            "https://ozon.ru/product/123",
            "https://wildberries.ru/catalog",
            "https://example.com/citilink", // не точное совпадение
            ""
        };

        foreach (var url in urls) {
            // Act
            var result = _parser.CanParse(url);

            // Assert
            Assert.False(result, $"URL: {url}");
        }
    }

    [Fact]
    public void CanParse_WhenUrlIsNull_ReturnsFalse() {
        // Act
        var result = _parser.CanParse(null!);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region ParsePriceAsync Tests

    [Fact]
    public async Task ParsePriceAsync_WhenValidHtmlWithPrice_ReturnsPrice() {
        // Arrange
        var url = "https://www.citilink.ru/product/test-product-123456/";
        var expectedPrice = 19999.99m;
        var html = $@"
            <html>
                <body>
                    <div data-meta-price=""{expectedPrice}"">Some content</div>
                </body>
            </html>";

        SetupHttpResponse(url, html);

        // Act
        var price = await _parser.ParsePriceAsync(url, CancellationToken.None);

        // Assert
        Assert.Equal(expectedPrice, price);
    }

    [Fact]
    public async Task ParsePriceAsync_WhenHtmlDoesNotContainPrice_ThrowsNotFoundException() {
        // Arrange
        var url = "https://www.citilink.ru/product/no-price/";
        var html = @"<html><body><div>No price here</div></body></html>";

        SetupHttpResponse(url, html);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _parser.ParsePriceAsync(url, CancellationToken.None));

        Assert.Contains("price", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ParsePriceAsync_WhenHtmlIsEmpty_ThrowsNotFoundException() {
        // Arrange
        var url = "https://www.citilink.ru/product/empty/";
        SetupHttpResponse(url, "");

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _parser.ParsePriceAsync(url, CancellationToken.None));
    }

    [Fact]
    public async Task ParsePriceAsync_WhenPriceIsZero_ReturnsZero() {
        // Arrange
        var url = "https://www.citilink.ru/product/free/";
        var expectedPrice = 0m;
        var html = $@"
            <html>
                <body>
                    <div data-meta-price=""{expectedPrice}"">Free</div>
                </body>
            </html>";

        SetupHttpResponse(url, html);

        // Act
        var price = await _parser.ParsePriceAsync(url, CancellationToken.None);

        // Assert
        Assert.Equal(expectedPrice, price);
    }

    [Fact]
    public async Task ParsePriceAsync_WhenHttpRequestFails_ThrowsHttpRequestException() {
        // Arrange
        var url = "https://www.citilink.ru/product/error/";
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            () => _parser.ParsePriceAsync(url, CancellationToken.None));
    }

    [Fact]
    public async Task ParsePriceAsync_WhenResponseIsTooManyRequests_ThrowsRateLimitException() {
        // Arrange
        var url = "https://www.citilink.ru/product/rate-limit/";
        SetupHttpResponse(url, "", HttpStatusCode.TooManyRequests);

        // Act & Assert
        await Assert.ThrowsAsync<RateLimitException>(
            () => _parser.ParsePriceAsync(url, CancellationToken.None));
    }

    [Fact]
    public async Task ParsePriceAsync_WhenResponseIsForbidden_ThrowsHttpRequestException() {
        // Arrange
        var url = "https://www.citilink.ru/product/forbidden/";
        SetupHttpResponse(url, "", HttpStatusCode.Forbidden);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            () => _parser.ParsePriceAsync(url, CancellationToken.None));
    }

    [Fact]
    public async Task ParsePriceAsync_WhenResponseIsNotFound_ThrowsNotFoundException() {
        // Arrange
        var url = "https://www.citilink.ru/product/not-found/";
        SetupHttpResponse(url, "", HttpStatusCode.NotFound);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _parser.ParsePriceAsync(url, CancellationToken.None));
    }

    #endregion

    #region ParseTitleAsync Tests

    [Fact]
    public async Task ParseTitleAsync_WhenValidHtmlWithTitle_ReturnsTitle() {
        // Arrange
        var url = "https://www.citilink.ru/product/test/";
        var expectedTitle = "Смартфон Apple iPhone 15 128GB Black";
        var html = $@"
            <html>
                <body>
                    <h1 class=""StyledProductTitle-sc-1q5c-1 dPpMPb"">{expectedTitle}</h1>
                </body>
            </html>";

        SetupHttpResponse(url, html);

        // Act
        var title = await _parser.ParseTitleAsync(url, CancellationToken.None);

        // Assert
        Assert.Equal(expectedTitle, title);
    }

    [Fact]
    public async Task ParseTitleAsync_WhenTitleContainsEscapeCharacters_ReturnsCleanedTitle() {
        // Arrange
        var url = "https://www.citilink.ru/product/escaped/";
        var expectedTitle = "Ноутбук Lenovo ThinkPad X1 Carbon";
        var html = $@"
            <html>
                <body>
                    <h1 class=""StyledProductTitle-sc-1q5c-1 dPpMPb"">{expectedTitle.Replace(" ", "\\ ")}</h1>
                </body>
            </html>";

        SetupHttpResponse(url, html);

        // Act
        var title = await _parser.ParseTitleAsync(url, CancellationToken.None);

        // Assert
        Assert.Equal(expectedTitle, title);
    }

    [Fact]
    public async Task ParseTitleAsync_WhenTitleHasWhitespace_Trimmed() {
        // Arrange
        var url = "https://www.citilink.ru/product/whitespace/";
        var rawTitle = "  Телевизор Samsung QLED 55\"  ";
        var expectedTitle = "Телевизор Samsung QLED 55\"";
        var html = $@"
            <html>
                <body>
                    <h1 class=""StyledProductTitle-sc-1q5c-1 dPpMPb"">{rawTitle}</h1>
                </body>
            </html>";

        SetupHttpResponse(url, html);

        // Act
        var title = await _parser.ParseTitleAsync(url, CancellationToken.None);

        // Assert
        Assert.Equal(expectedTitle, title);
    }

    [Fact]
    public async Task ParseTitleAsync_WhenHtmlDoesNotContainTitle_ThrowsNotFoundException() {
        // Arrange
        var url = "https://www.citilink.ru/product/no-title/";
        var html = @"<html><body><div>No title element here</div></body></html>";

        SetupHttpResponse(url, html);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _parser.ParseTitleAsync(url, CancellationToken.None));

        Assert.Contains("Title", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ParseTitleAsync_WhenHtmlIsEmpty_ThrowsNotFoundException() {
        // Arrange
        var url = "https://www.citilink.ru/product/empty-title/";
        SetupHttpResponse(url, "");

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _parser.ParseTitleAsync(url, CancellationToken.None));
    }

    [Fact]
    public async Task ParseTitleAsync_WhenTitleContainsHTMLSpecialChars_ReturnsDecodedTitle() {
        // Arrange
        var url = "https://www.citilink.ru/product/special/";
        var expectedTitle = "Наушники & колонки Sony & LG";
        var html = $@"
            <html>
                <body>
                    <h1 class=""StyledProductTitle-sc-1q5c-1 dPpMPb"">{expectedTitle}</h1>
                </body>
            </html>";

        SetupHttpResponse(url, html);

        // Act
        var title = await _parser.ParseTitleAsync(url, CancellationToken.None);

        // Assert
        Assert.Equal(expectedTitle, title);
    }

    #endregion

    #region FetchHtmlAsync Tests

    [Fact]
    public async Task FetchHtmlAsync_WhenSuccessfulRequest_ReturnsHtmlContent() {
        // Arrange
        var url = "https://www.citilink.ru/product/test/";
        var expectedHtml = "<html><body>Test content</body></html>";
        SetupHttpResponse(url, expectedHtml);

        // Act
        var html = await _parser.FetchHtmlAsync(url, CancellationToken.None);

        // Assert
        Assert.Equal(expectedHtml, html);
    }

    [Fact]
    public async Task FetchHtmlAsync_WhenResponseHasGzipEncoding_DecompressesSuccessfully() {
        // Arrange
        var url = "https://www.citilink.ru/product/gzip/";
        var originalHtml = "<html><body>Gzipped content</body></html>";
        var compressedBytes = await GzipCompressAsync(originalHtml);

        SetupHttpResponseWithEncoding(url, compressedBytes, "gzip");

        // Act
        var html = await _parser.FetchHtmlAsync(url, CancellationToken.None);

        // Assert
        Assert.Equal(originalHtml, html);
    }

    [Fact]
    public async Task FetchHtmlAsync_WhenRequestCancelled_ThrowsOperationCanceledException() {
        // Arrange
        var url = "https://www.citilink.ru/product/cancel/";
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Returns<HttpRequestMessage, CancellationToken>(async (req, ct) => {
                ct.ThrowIfCancellationRequested();
                return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(
            () => _parser.ParsePriceAsync(url, cts.Token));
    }

    #endregion

    #region UserAgent and Headers Tests

    [Fact]
    public void Constructor_SetsCorrectUserAgent() {
        // Assert
        Assert.Contains("Mozilla/5.0", _httpClient.DefaultRequestHeaders.UserAgent.ToString());
        Assert.Contains("Chrome/120.0.0.0", _httpClient.DefaultRequestHeaders.UserAgent.ToString());
    }

    [Fact]
    public void Constructor_SetsAcceptHeader() {
        // Assert
        var acceptHeader = _httpClient.DefaultRequestHeaders.Accept.ToString();
        Assert.Contains("text/html", acceptHeader);
        Assert.Contains("application/xhtml+xml", acceptHeader);
    }

    [Fact]
    public void Constructor_SetsAcceptLanguageHeader() {
        // Assert
        var acceptLanguage = _httpClient.DefaultRequestHeaders.AcceptLanguage.ToString();
        Assert.Contains("ru-RU", acceptLanguage);
        Assert.Contains("ru", acceptLanguage);
    }

    [Fact]
    public void Constructor_SetsAcceptEncodingHeader() {
        // Assert
        var acceptEncoding = _httpClient.DefaultRequestHeaders.AcceptEncoding.ToString();
        Assert.Contains("gzip", acceptEncoding);
        Assert.Contains("deflate", acceptEncoding);
    }

    #endregion

    #region Helper Methods

    private void SetupHttpResponse(string url, string content, HttpStatusCode statusCode = HttpStatusCode.OK) {
        var response = new HttpResponseMessage(statusCode) {
            Content = new StringContent(content, Encoding.UTF8, "text/html")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString() == url),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    }

    private void SetupHttpResponseWithEncoding(string url, byte[] content, string encoding) {
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Content = new ByteArrayContent(content);
        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
        response.Content.Headers.ContentEncoding.Add(encoding);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString() == url),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    }

    private static async Task<byte[]> GzipCompressAsync(string text) {
        using var memoryStream = new MemoryStream();
        using (var gzipStream = new System.IO.Compression.GZipStream(memoryStream, System.IO.Compression.CompressionMode.Compress, leaveOpen: true))
        using (var writer = new StreamWriter(gzipStream, Encoding.UTF8)) {
            await writer.WriteAsync(text);
        }
        return memoryStream.ToArray();
    }

    #endregion
}