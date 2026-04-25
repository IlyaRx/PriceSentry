using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using PriceSentry.Application.Common.Exceptions;
using PriceSentry.Application.Interfaces;
using PriceSentry.Application.Product.Commands.Update;
using PriceSentry.Domain;
using PriceSentry.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PriceSentry.Tests.Handlers {
    public class UpdateProductCommandHandlerTests {

        private IPriceSentryDbContext CreateDbContext() {
            var options = new DbContextOptionsBuilder<PriceSentryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new PriceSentryDbContext(options);
        }


        [Fact]
        public async Task Handle_WhenUserOwnsProduct_UpdatesDesiredPrice() {
            // Arrange
            IPriceSentryDbContext dbContextMemory = CreateDbContext();
            var handler = new UpdateProductCommandHandler(dbContextMemory);
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var newDesiredPrice = 1500m;
            var user = new ApplicationUser { Id = userId, Email = "test@example.com" };
            var product = new TrackingProduct {
                Id = productId,
                UserId = userId,
                ProductUrl = "https://example.com/product",
                DesiredPrice = 10000,
                User = user,
            };
            var command = new UpdateProductCommand {
                UserId = userId,
                Id = productId,
                DesiredPrice = newDesiredPrice
            };

            await dbContextMemory.Users.AddAsync(user, CancellationToken.None);
            await dbContextMemory.Products.AddAsync(product, CancellationToken.None);
            await dbContextMemory.SaveChangesAsync(CancellationToken.None);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(newDesiredPrice, product.DesiredPrice);

        }

        [Fact]
        public async Task Handle_WhenProductNotFound_ThrowsNotFoundException() {
            // Arrange
            IPriceSentryDbContext dbContextMemory = CreateDbContext();
            var handler = new UpdateProductCommandHandler(dbContextMemory);
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var command = new UpdateProductCommand {
                UserId = userId,
                Id = productId,
                DesiredPrice = 1000m
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => handler.Handle(command, CancellationToken.None));


        }

        [Fact]
        public async Task Handle_WhenUserDoesNotOwnProduct_ThrowsNotFoundException() {
            // Arrange
            IPriceSentryDbContext dbContextMemory = CreateDbContext();
            var handler = new UpdateProductCommandHandler(dbContextMemory);
            var ownerId = Guid.NewGuid();
            var tryingUserId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var user = new ApplicationUser { Id = ownerId, Email = "test@example.com" };
            var product = new TrackingProduct {
                Id = productId,
                UserId = ownerId,
                ProductUrl = "https://example.com/product",
                DesiredPrice = 1000m,
                User = user,
            };
            var command = new UpdateProductCommand {
                UserId = tryingUserId,
                Id = productId,
                DesiredPrice = 2000m
            };
            await dbContextMemory.Users.AddAsync(user, CancellationToken.None);
            await dbContextMemory.Products.AddAsync(product, CancellationToken.None);
            await dbContextMemory.SaveChangesAsync(CancellationToken.None);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => handler.Handle(command, CancellationToken.None));

            Assert.Equal(1000m, product.DesiredPrice);
            
        }

        [Fact]
        public async Task Handle_UpdatesPriceToZero_IsAllowed() {
            // Arrange
            IPriceSentryDbContext dbContextMemory = CreateDbContext();
            var handler = new UpdateProductCommandHandler(dbContextMemory);
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
            var command = new UpdateProductCommand {
                UserId = userId,
                Id = productId,
                DesiredPrice = 0m
            };
            await dbContextMemory.Users.AddAsync(user, CancellationToken.None);
            await dbContextMemory.Products.AddAsync(product, CancellationToken.None);
            await dbContextMemory.SaveChangesAsync(CancellationToken.None);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(0m, product.DesiredPrice);
        }

        [Fact]
        public async Task Handle_UpdatesPriceToMaximum_IsAllowed() {
            // Arrange
            IPriceSentryDbContext dbContextMemory = CreateDbContext();
            var handler = new UpdateProductCommandHandler(dbContextMemory);
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
            var command = new UpdateProductCommand {
                UserId = userId,
                Id = productId,
                DesiredPrice = 1_000_000m
            };
            await dbContextMemory.Users.AddAsync(user, CancellationToken.None);
            await dbContextMemory.Products.AddAsync(product, CancellationToken.None);
            await dbContextMemory.SaveChangesAsync(CancellationToken.None);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(1_000_000m, product.DesiredPrice);
        }
    }
}