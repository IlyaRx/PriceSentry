using Microsoft.EntityFrameworkCore;
using PriceSentry.Application.Common.Exceptions;
using PriceSentry.Application.Interfaces;
using PriceSentry.Application.Product.Commands.Delete;
using PriceSentry.Domain;
using PriceSentry.Persistence;
using Xunit;

namespace PriceSentry.Tests.Handlers {
    public class DeleteProductCommandHandlerTests {

        private IPriceSentryDbContext CreateDbContext() {
            var options = new DbContextOptionsBuilder<PriceSentryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new PriceSentryDbContext(options);
        }

        [Fact]
        public async Task Handle_WhenUserOwnsProduct_DeletesProduct() {
            // Arrange
            IPriceSentryDbContext dbContextMemory = CreateDbContext();
            var handler = new DeleteProductCommandHandler(dbContextMemory);
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId, Email = "test@example.com" };
            var product = new TrackingProduct {
                Id = productId,
                UserId = userId,
                ProductUrl = "https://example.com/product",
                DesiredPrice = 10000,
                User = user,
            };
            var command = new DeleteProductCommand { UserId = userId, Id = productId };

            await dbContextMemory.Users.AddAsync(user, CancellationToken.None);
            await dbContextMemory.Products.AddAsync(product, CancellationToken.None);
            await dbContextMemory.SaveChangesAsync(CancellationToken.None);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var productDel = await dbContextMemory.Products.FirstOrDefaultAsync(p => p.Id == productId, CancellationToken.None);
            var count = await dbContextMemory.Products.CountAsync(CancellationToken.None);
            Assert.Null(productDel);
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task Handle_WhenProductNotFound_ThrowsNotFoundException() {
            // Arrange
            IPriceSentryDbContext dbContextMemory = CreateDbContext();
            var handler = new DeleteProductCommandHandler(dbContextMemory);
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var command = new DeleteProductCommand { UserId = userId, Id = productId };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotOwnProduct_ThrowsNotFoundException() {
            // Arrange
            IPriceSentryDbContext dbContextMemory = CreateDbContext();
            var handler = new DeleteProductCommandHandler(dbContextMemory);
            var ownerId = Guid.NewGuid();
            var tryingUserId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var user = new ApplicationUser { Id = ownerId, Email = "test@example.com" };
            var product = new TrackingProduct {
                Id = productId,
                UserId = ownerId,
                ProductUrl = "https://example.com/product",
                DesiredPrice = 10000,
                User = user,
            };
            var command = new DeleteProductCommand { UserId = tryingUserId, Id = productId };
            await dbContextMemory.Users.AddAsync(user, CancellationToken.None);
            await dbContextMemory.Products.AddAsync(product, CancellationToken.None);
            await dbContextMemory.SaveChangesAsync(CancellationToken.None);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => handler.Handle(command, CancellationToken.None));

        }

        [Fact]
        public async Task Handle_WithEmptyGuid_ShouldStillTryToFind() {
            // Arrange
            IPriceSentryDbContext dbContextMemory = CreateDbContext();
            var handler = new DeleteProductCommandHandler(dbContextMemory);
            var command = new DeleteProductCommand { UserId = Guid.NewGuid(), Id = Guid.Empty };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => handler.Handle(command, CancellationToken.None));
        }
    }
}