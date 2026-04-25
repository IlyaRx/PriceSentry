using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PriceSentry.Application.Interfaces;
using PriceSentry.Application.Price.Queries.GetPriceHistoryList;
using PriceSentry.Domain;
using PriceSentry.Persistence;
using PriceSentry.Tests.Mappings;
using Xunit;

namespace PriceSentry.Tests.Handlers;

public class GetPriceHistoryQueryHandlerTests {
    private readonly IMapper _mapper;

    public GetPriceHistoryQueryHandlerTests() {
        var config = new MapperConfiguration(cfg =>
            cfg.AddProfile<TestMappingProfile>(), NullLoggerFactory.Instance);
        _mapper = config.CreateMapper();
    }

    private IPriceSentryDbContext CreateDbContext() {
        var options = new DbContextOptionsBuilder<PriceSentryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PriceSentryDbContext(options);
    }

    [Fact]
    public async Task Handle_ReturnsAllPriceHistoryForProduct() {
        // Arrange
        var dbContext = CreateDbContext();
        var handler = new GetPriceHistoryQueryHandler(_mapper, dbContext);
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var user = new ApplicationUser { Id = userId, Email = "test@example.com" };
        var product = new TrackingProduct {
            Id = productId,
            UserId = userId,
            ProductUrl = "https://example.com/product",
            DesiredPrice = 1000,
            User = user
        };

        var priceHistory = new List<ProductPriceHistory>
        {
            new() { Id = Guid.NewGuid(), ProductId = productId, Price = 1000, AddDate = DateTime.UtcNow.AddDays(-2), TrackingProduct = product },
            new() { Id = Guid.NewGuid(), ProductId = productId, Price = 900, AddDate = DateTime.UtcNow.AddDays(-1), TrackingProduct = product },
            new() { Id = Guid.NewGuid(), ProductId = productId, Price = 850, AddDate = DateTime.UtcNow, TrackingProduct = product }
        };

        await dbContext.Users.AddAsync(user, CancellationToken.None);
        await dbContext.Products.AddAsync(product, CancellationToken.None);
        await dbContext.ProductPrices.AddRangeAsync(priceHistory, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new GetPriceHistoryQuery { ProductId = productId, UserId = userId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Prices);
        Assert.Equal(3, result.Prices.Count);
        Assert.Contains(result.Prices, p => p.Price == 1000);
        Assert.Contains(result.Prices, p => p.Price == 900);
        Assert.Contains(result.Prices, p => p.Price == 850);
    }

    [Fact]
    public async Task Handle_WhenNoPriceHistory_ReturnsEmptyList() {
        // Arrange
        var dbContext = CreateDbContext();
        var handler = new GetPriceHistoryQueryHandler(_mapper, dbContext);
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var user = new ApplicationUser { Id = userId, Email = "test@example.com" };
        var product = new TrackingProduct {
            Id = productId,
            UserId = userId,
            ProductUrl = "https://example.com/product",
            DesiredPrice = 1000,
            User = user
        };

        await dbContext.Users.AddAsync(user, CancellationToken.None);
        await dbContext.Products.AddAsync(product, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new GetPriceHistoryQuery { ProductId = productId, UserId = userId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Prices);
        Assert.Empty(result.Prices);
    }

    [Fact]
    public async Task Handle_ReturnsPricesInChronologicalOrder() {
        // Arrange
        var dbContext = CreateDbContext();
        var handler = new GetPriceHistoryQueryHandler(_mapper, dbContext);
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var user = new ApplicationUser { Id = userId, Email = "test@example.com" };
        var product = new TrackingProduct {
            Id = productId,
            UserId = userId,
            ProductUrl = "https://example.com/product",
            DesiredPrice = 1000,
            User = user
        };

        var now = DateTime.UtcNow;
        var priceHistory = new List<ProductPriceHistory>
        {
            new() { Id = Guid.NewGuid(), ProductId = productId, Price = 1000, AddDate = now.AddDays(-2), TrackingProduct = product },
            new() { Id = Guid.NewGuid(), ProductId = productId, Price = 900, AddDate = now.AddDays(-1), TrackingProduct = product },
            new() { Id = Guid.NewGuid(), ProductId = productId, Price = 800, AddDate = now, TrackingProduct = product }
        };

        await dbContext.Users.AddAsync(user, CancellationToken.None);
        await dbContext.Products.AddAsync(product, CancellationToken.None);
        await dbContext.ProductPrices.AddRangeAsync(priceHistory, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new GetPriceHistoryQuery { ProductId = productId, UserId = userId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Prices);
        var prices = result.Prices.ToList();
        for (int i = 0; i < prices.Count - 1; i++) {
            Assert.True(prices[i].AddDate <= prices[i + 1].AddDate);
        }
    }
}