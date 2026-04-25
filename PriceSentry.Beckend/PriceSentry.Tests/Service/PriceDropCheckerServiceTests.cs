using PriceSentry.Application.Sepvices;
using PriceSentry.Domain;
using Xunit;

namespace PriceSentry.Tests.Service;

public class PriceDropCheckerServiceTests {
    private readonly PriceDropCheckerService _service;

    public PriceDropCheckerServiceTests() {
        _service = new PriceDropCheckerService();
    }

    [Fact]
    public void ShouldNotify_WhenNewPriceIsLessThanDesired_ReturnsTrue() {
        // Arrange
        Guid userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId, Email = "test@example.com" };
        var product = new TrackingProduct {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductUrl = "https://example.com",
            DesiredPrice = 1000,
            User = user
        };
        var newPrice = 999m;

        // Act
        var result = _service.ShouldNotify(product, newPrice);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldNotify_WhenNewPriceEqualsDesired_ReturnsTrue() {
        // Arrange
        Guid userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId, Email = "test@example.com" };
        var product = new TrackingProduct {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductUrl = "https://example.com",
            DesiredPrice = 1000,
            User = user
        };
        var newPrice = 1000m;

        // Act
        var result = _service.ShouldNotify(product, newPrice);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldNotify_WhenNewPriceIsGreaterThanDesired_ReturnsFalse() {
        // Arrange
        Guid userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId, Email = "test@example.com" };
        var product = new TrackingProduct {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductUrl = "https://example.com",
            DesiredPrice = 1000,
            User = user
        };
        var newPrice = 1001m;

        // Act
        var result = _service.ShouldNotify(product, newPrice);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(500, 499)]
    [InlineData(500, 500)]
    [InlineData(0, 0)]
    [InlineData(1000000, 999999)]
    public void ShouldNotify_WithVariousPrices_ReturnsExpected(decimal desired, decimal newPrice) {
        // Arrange
        Guid userId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId, Email = "test@example.com" };
        var product = new TrackingProduct {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductUrl = "https://example.com",
            DesiredPrice = desired,
            User = user
        };

        // Act
        var result = _service.ShouldNotify(product, newPrice);

        // Assert
        Assert.Equal(newPrice <= desired, result);
    }
}