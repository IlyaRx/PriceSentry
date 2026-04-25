using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PriceSentry.Application.Interfaces;
using PriceSentry.Application.Product.Queries.GetListProducts;
using PriceSentry.Domain;
using PriceSentry.Persistence;
using PriceSentry.Tests.Mappings;
using Xunit;

namespace PriceSentry.Tests.Handlers;

public class ProductListQueryHandlerTests {
    private readonly IMapper _mapper;

    public ProductListQueryHandlerTests() {
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
    public async Task Handle_ReturnsAllProductsForUser() {
        // Arrange
        var dbContext = CreateDbContext();
        var handler = new ProductListQueryHundler(dbContext, _mapper);
        var userId = Guid.NewGuid();

        var user = new ApplicationUser { Id = userId, Email = "test@example.com" };
        var products = new List<TrackingProduct>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, ProductUrl = "https://example.com/1", DesiredPrice = 1000, ActualPrice = 900, Title = "Product 1", User = user },
            new() { Id = Guid.NewGuid(), UserId = userId, ProductUrl = "https://example.com/2", DesiredPrice = 2000, ActualPrice = 1800, Title = "Product 2", User = user },
            new() { Id = Guid.NewGuid(), UserId = userId, ProductUrl = "https://example.com/3", DesiredPrice = 3000, ActualPrice = 2500, Title = "Product 3", User = user }
        };

        await dbContext.Users.AddAsync(user, CancellationToken.None);
        await dbContext.Products.AddRangeAsync(products, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new ProductListQuery { UserId = userId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result.ProductList);
        Assert.Equal(3, result.ProductList.Count);
    }

    [Fact]
    public async Task Handle_WhenUserHasNoProducts_ReturnsEmptyList() {
        // Arrange
        var dbContext = CreateDbContext();
        var handler = new ProductListQueryHundler(dbContext, _mapper);
        var userId = Guid.NewGuid();

        var user = new ApplicationUser { Id = userId, Email = "test@example.com" };
        await dbContext.Users.AddAsync(user, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new ProductListQuery { UserId = userId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result.ProductList);
        Assert.Empty(result.ProductList);
    }

    [Fact]
    public async Task Handle_DoesNotReturnOtherUsersProducts() {
        // Arrange
        var dbContext = CreateDbContext();
        var handler = new ProductListQueryHundler(dbContext, _mapper);
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var user1 = new ApplicationUser { Id = userId1, Email = "user1@example.com" };
        var user2 = new ApplicationUser { Id = userId2, Email = "user2@example.com" };

        var products = new List<TrackingProduct>
        {
            new() { Id = Guid.NewGuid(), UserId = userId1, ProductUrl = "https://example.com/1", DesiredPrice = 1000, Title = "User1 Product", User = user1 },
            new() { Id = Guid.NewGuid(), UserId = userId2, ProductUrl = "https://example.com/2", DesiredPrice = 2000, Title = "User2 Product", User = user2 }
        };

        await dbContext.Users.AddRangeAsync(user1, user2);
        await dbContext.Products.AddRangeAsync(products, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new ProductListQuery { UserId = userId1 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result.ProductList);
        Assert.Single(result.ProductList);
        Assert.Equal("User1 Product", result.ProductList.First().Title);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectProductProperties() {
        // Arrange
        var dbContext = CreateDbContext();
        var handler = new ProductListQueryHundler(dbContext, _mapper);
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var user = new ApplicationUser { Id = userId, Email = "test@example.com" };
        var product = new TrackingProduct {
            Id = productId,
            UserId = userId,
            ProductUrl = "https://example.com/product",
            DesiredPrice = 1500,
            ActualPrice = 1200,
            Title = "Awesome Product",
            User = user
        };

        await dbContext.Users.AddAsync(user, CancellationToken.None);
        await dbContext.Products.AddAsync(product, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new ProductListQuery { UserId = userId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        var productVm = result.ProductList!.First();
        Assert.Equal(productId, productVm.Id);
        Assert.Equal("Awesome Product", productVm.Title);
        Assert.Equal("https://example.com/product", productVm.ProductUrl);
        Assert.Equal(1200, productVm.ActualPrice);
        Assert.Equal(1500, productVm.DesiredPrice);
    }
}