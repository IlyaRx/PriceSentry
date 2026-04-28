using Microsoft.Extensions.Logging;
using Moq;
using PriceSentry.Persistence.Interfases;
using PriceSentry.Persistence.Providers;
using Xunit;

namespace PriceSentry.Tests.Providers;

public class PriceParserProviderTests {
    private readonly Mock<ILogger<PriceParserProvider>> _loggerMock;
    private readonly List<Mock<IShopPriceParser>> _parserMocks;
    private readonly PriceParserProvider _provider;

    public PriceParserProviderTests() {
        _loggerMock = new Mock<ILogger<PriceParserProvider>>();
        _parserMocks = new List<Mock<IShopPriceParser>>();
        _provider = new PriceParserProvider(
            _parserMocks.Select(m => m.Object),
            _loggerMock.Object);
    }

    #region GetPriceAsync Tests

    [Fact]
    public async Task GetPriceAsync_WhenParserFound_ReturnsPrice() {
        // Arrange
        var url = "https://www.citilink.ru/product/test/";
        var expectedPrice = 15999m;

        var mockParser = new Mock<IShopPriceParser>();
        mockParser.Setup(p => p.CanParse(url)).Returns(true);
        mockParser.Setup(p => p.ParsePriceAsync(url, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPrice);

        var provider = new PriceParserProvider(new[] { mockParser.Object }, _loggerMock.Object);

        // Act
        var price = await provider.GetPriceAsync(url, CancellationToken.None);

        // Assert
        Assert.Equal(expectedPrice, price);
        mockParser.Verify(p => p.ParsePriceAsync(url, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPriceAsync_WhenMultipleParsers_PicksFirstThatCanParse() {
        // Arrange
        var url = "https://www.citilink.ru/product/test/";
        var expectedPrice = 12999m;

        var mockParser1 = new Mock<IShopPriceParser>();
        var mockParser2 = new Mock<IShopPriceParser>();
        var mockParser3 = new Mock<IShopPriceParser>();

        mockParser1.Setup(p => p.CanParse(url)).Returns(false);
        mockParser2.Setup(p => p.CanParse(url)).Returns(true);
        mockParser2.Setup(p => p.ParsePriceAsync(url, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPrice);
        mockParser3.Setup(p => p.CanParse(url)).Returns(true);

        var provider = new PriceParserProvider(
            new[] { mockParser1.Object, mockParser2.Object, mockParser3.Object },
            _loggerMock.Object);

        // Act
        var price = await provider.GetPriceAsync(url, CancellationToken.None);

        // Assert
        Assert.Equal(expectedPrice, price);
        mockParser2.Verify(p => p.ParsePriceAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        mockParser3.Verify(p => p.ParsePriceAsync(url, It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetPriceAsync_WhenNoParserFound_ThrowsNotSupportedException() {
        // Arrange
        var url = "https://unknown-shop.com/product/";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotSupportedException>(
            () => _provider.GetPriceAsync(url, CancellationToken.None));

        Assert.Contains(url, exception.Message);
    }

    [Fact]
    public async Task GetPriceAsync_WhenParserThrowsException_PropagatesException() {
        // Arrange
        var url = "https://www.citilink.ru/product/error/";
        var expectedException = new InvalidOperationException("Parser error");

        var mockParser = new Mock<IShopPriceParser>();
        mockParser.Setup(p => p.CanParse(url)).Returns(true);
        mockParser.Setup(p => p.ParsePriceAsync(url, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var provider = new PriceParserProvider(new[] { mockParser.Object }, _loggerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => provider.GetPriceAsync(url, CancellationToken.None));

        Assert.Equal("Parser error", exception.Message);
    }

    [Fact]
    public async Task GetPriceAsync_WhenParserReturnsZero_ReturnsZero() {
        // Arrange
        var url = "https://www.citilink.ru/product/free/";
        var expectedPrice = 0m;

        var mockParser = new Mock<IShopPriceParser>();
        mockParser.Setup(p => p.CanParse(url)).Returns(true);
        mockParser.Setup(p => p.ParsePriceAsync(url, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPrice);

        var provider = new PriceParserProvider(new[] { mockParser.Object }, _loggerMock.Object);

        // Act
        var price = await provider.GetPriceAsync(url, CancellationToken.None);

        // Assert
        Assert.Equal(expectedPrice, price);
    }

    #endregion

    #region GetTitleAsync Tests

    [Fact]
    public async Task GetTitleAsync_WhenParserFound_ReturnsTitle() {
        // Arrange
        var url = "https://www.citilink.ru/product/test/";
        var expectedTitle = "Test Product Title";

        var mockParser = new Mock<IShopPriceParser>();
        mockParser.Setup(p => p.CanParse(url)).Returns(true);
        mockParser.Setup(p => p.ParseTitleAsync(url, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTitle);

        var provider = new PriceParserProvider(new[] { mockParser.Object }, _loggerMock.Object);

        // Act
        var title = await provider.GetTitleAsync(url, CancellationToken.None);

        // Assert
        Assert.Equal(expectedTitle, title);
        mockParser.Verify(p => p.ParseTitleAsync(url, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTitleAsync_WhenMultipleParsers_PicksFirstThatCanParse() {
        // Arrange
        var url = "https://www.citilink.ru/product/test/";
        var expectedTitle = "Selected Product";

        var mockParser1 = new Mock<IShopPriceParser>();
        var mockParser2 = new Mock<IShopPriceParser>();

        mockParser1.Setup(p => p.CanParse(url)).Returns(false);
        mockParser2.Setup(p => p.CanParse(url)).Returns(true);
        mockParser2.Setup(p => p.ParseTitleAsync(url, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTitle);

        var provider = new PriceParserProvider(
            new[] { mockParser1.Object, mockParser2.Object },
            _loggerMock.Object);

        // Act
        var title = await provider.GetTitleAsync(url, CancellationToken.None);

        // Assert
        Assert.Equal(expectedTitle, title);
    }

    [Fact]
    public async Task GetTitleAsync_WhenNoParserFound_ThrowsNotSupportedException() {
        // Arrange
        var url = "https://unknown-shop.com/product/";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotSupportedException>(
            () => _provider.GetTitleAsync(url, CancellationToken.None));

        Assert.Contains(url, exception.Message);
    }

    [Fact]
    public async Task GetTitleAsync_WhenParserThrowsException_PropagatesException() {
        // Arrange
        var url = "https://www.citilink.ru/product/error/";
        var expectedException = new InvalidOperationException("Title parser error");

        var mockParser = new Mock<IShopPriceParser>();
        mockParser.Setup(p => p.CanParse(url)).Returns(true);
        mockParser.Setup(p => p.ParseTitleAsync(url, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var provider = new PriceParserProvider(new[] { mockParser.Object }, _loggerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => provider.GetTitleAsync(url, CancellationToken.None));

        Assert.Equal("Title parser error", exception.Message);
    }

    [Fact]
    public async Task GetTitleAsync_WhenParserReturnsEmptyString_ReturnsEmptyString() {
        // Arrange
        var url = "https://www.citilink.ru/product/empty-title/";
        var expectedTitle = string.Empty;

        var mockParser = new Mock<IShopPriceParser>();
        mockParser.Setup(p => p.CanParse(url)).Returns(true);
        mockParser.Setup(p => p.ParseTitleAsync(url, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTitle);

        var provider = new PriceParserProvider(new[] { mockParser.Object }, _loggerMock.Object);

        // Act
        var title = await provider.GetTitleAsync(url, CancellationToken.None);

        // Assert
        Assert.Equal(expectedTitle, title);
    }

    #endregion

    #region Cancellation Tests

    [Fact]
    public async Task GetPriceAsync_WhenCancelled_ThrowsOperationCanceledException() {
        // Arrange
        var url = "https://www.citilink.ru/product/cancel/";
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var mockParser = new Mock<IShopPriceParser>();
        mockParser.Setup(p => p.CanParse(url)).Returns(true);
        mockParser.Setup(p => p.ParsePriceAsync(url, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var provider = new PriceParserProvider(new[] { mockParser.Object }, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => provider.GetPriceAsync(url, cts.Token));
    }

    [Fact]
    public async Task GetTitleAsync_WhenCancelled_ThrowsOperationCanceledException() {
        // Arrange
        var url = "https://www.citilink.ru/product/cancel/";
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var mockParser = new Mock<IShopPriceParser>();
        mockParser.Setup(p => p.CanParse(url)).Returns(true);
        mockParser.Setup(p => p.ParseTitleAsync(url, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var provider = new PriceParserProvider(new[] { mockParser.Object }, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => provider.GetTitleAsync(url, cts.Token));
    }

    #endregion
}