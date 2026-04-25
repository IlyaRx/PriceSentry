using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PriceSentry.Application.Common.Exceptions;
using PriceSentry.Application.Interfaces;
using PriceSentry.Application.Product.Queries.GetActualPrice;
using PriceSentry.Domain;
using PriceSentry.Persistence;
using PriceSentry.Tests.Mappings;
using Xunit;

namespace PriceSentry.Tests.Handlers;

public class GetActualPriceQueryHandlerTests {
    private readonly IMapper _mapper;
    public GetActualPriceQueryHandlerTests() {
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
    public async Task Handle_WhenUserOwnsProduct_ReturnsActualPrice() {
        // Arrange
        var dbContext = CreateDbContext();
        var handler = new GetActualPriceQueryHandler(dbContext, _mapper);
        var userId = Guid.NewGuid(); 
        var productId = Guid.NewGuid();
        var expectedPrice = 999m;

        var user = new ApplicationUser { Id = userId, Email = "test@example.com" };
        var product = new TrackingProduct {
            Id = productId,
            UserId = userId,
            ProductUrl = "https://example.com/product",
            DesiredPrice = 1000,
            ActualPrice = expectedPrice,
            User = user
        };

        await dbContext.Users.AddAsync(user, CancellationToken.None);
        await dbContext.Products.AddAsync(product, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new GetActualPriceQuery { Id = productId, UserId = userId };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(productId, result.Id);
        Assert.Equal(expectedPrice, result.ActualPrice);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ThrowsNotFoundException() {
        // Arrange
        var dbContext = CreateDbContext();
        var handler = new GetActualPriceQueryHandler(dbContext, _mapper);
        var query = new GetActualPriceQuery {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotOwnProduct_ThrowsNotFoundException() {
        // Arrange
        var dbContext = CreateDbContext();
        var handler = new GetActualPriceQueryHandler(dbContext, _mapper);
        var ownerId = Guid.NewGuid();
        var tryingUserId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var user = new ApplicationUser { Id = ownerId, Email = "owner@example.com" };
        var product = new TrackingProduct {
            Id = productId,
            UserId = ownerId,
            ProductUrl = "https://example.com/product",
            DesiredPrice = 1000,
            ActualPrice = 500,
            User = user
        };

        await dbContext.Users.AddAsync(user, CancellationToken.None);
        await dbContext.Products.AddAsync(product, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var query = new GetActualPriceQuery { Id = productId, UserId = tryingUserId };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(query, CancellationToken.None));
    }
}