using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using PriceSentry.Application.Interfaces;
using PriceSentry.Application.Product.Commands.Create;
using PriceSentry.Domain;
using PriceSentry.Persistence;
using Xunit;

namespace PriceSentry.Tests.Handlers {
    public class CreateProductCommandHandlerTests {
        private readonly Mock<IProductPriceProvider> _priceProviderMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

        public CreateProductCommandHandlerTests() {
            _priceProviderMock = new Mock<IProductPriceProvider>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        }

        private IPriceSentryDbContext CreateDbContext() {
            var options = new DbContextOptionsBuilder<PriceSentryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new PriceSentryDbContext(options);
        }

        [Fact]
        public async Task Handle_WhenProductDoesNotExist_CreatesNewProduct() {
            // Arrange
            IPriceSentryDbContext _dbContextMemoery = CreateDbContext();
            var _handler = new CreateProductCommandHandler(
                _dbContextMemoery,
                _priceProviderMock.Object,
                _userManagerMock.Object);

            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId, Email = "test@example.com" };
            var command = new CreateProductCommand {
                UserId = userId,
                DesiredPrice = 1000,
                ProductUrl = "https://example.com/product"
            };
            var expectedPrice = 999m;
            var expectedTitle = "Test Product";

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            _priceProviderMock.Setup(x => x.GetPriceAsync(command.ProductUrl, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedPrice);
            _priceProviderMock.Setup(x => x.GetTitleAsync(command.ProductUrl, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTitle);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
            var savedProduct = await _dbContextMemoery.Products.FirstOrDefaultAsync(p => p.ProductUrl == command.ProductUrl, CancellationToken.None);
            Assert.NotNull(savedProduct);  
            Assert.Equal(expectedPrice, savedProduct.ActualPrice);
        }

        [Fact]
        public async Task Handle_WhenProductAlreadyExists_ReturnsExistingProductId() {
            // Arrange
            IPriceSentryDbContext _dbContextMemoery = CreateDbContext();
            var _handler = new CreateProductCommandHandler(
                _dbContextMemoery,
                _priceProviderMock.Object,
                _userManagerMock.Object);
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId, Email = "test@example.com" };
            var existingProductId = Guid.NewGuid();
            var existingProduct = new TrackingProduct {
                Id = existingProductId,
                UserId = userId,
                ProductUrl = "https://example.com/product",
                DesiredPrice = 10000,
                User = user,
            };
            var command = new CreateProductCommand {
                UserId = userId,
                DesiredPrice = 1000,
                ProductUrl = existingProduct.ProductUrl
            };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            await _dbContextMemoery.Products.AddAsync(existingProduct, CancellationToken.None);
            await _dbContextMemoery.SaveChangesAsync(CancellationToken.None);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            var count = await _dbContextMemoery.Products.CountAsync(CancellationToken.None);
            Assert.Equal(existingProductId, result);
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task Handle_ParallelRequests_GetPriceAndTitleConcurrently() {
            // Arrange
            IPriceSentryDbContext _dbContextMemoery = CreateDbContext();
            var _handler = new CreateProductCommandHandler(
                _dbContextMemoery,
                _priceProviderMock.Object,
                _userManagerMock.Object);
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId, Email = "test@example.com" };
            var command = new CreateProductCommand {
                UserId = userId,
                DesiredPrice = 500,
                ProductUrl = "https://example.com/parallel"
            };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);


            var priceTask = Task.Delay(100, CancellationToken.None).ContinueWith(_ => 1000m);
            var titleTask = Task.Delay(50, CancellationToken.None).ContinueWith(_ => "Product Title");

            _priceProviderMock.Setup(x => x.GetPriceAsync(command.ProductUrl, It.IsAny<CancellationToken>()))
                .Returns(priceTask);
            _priceProviderMock.Setup(x => x.GetTitleAsync(command.ProductUrl, It.IsAny<CancellationToken>()))
                .Returns(titleTask);

            // Act
            var startTime = DateTime.UtcNow;
            await _handler.Handle(command, CancellationToken.None);
            var elapsed = DateTime.UtcNow - startTime;

            Assert.True(elapsed.TotalMilliseconds < 150, $"Elapsed time: {elapsed.TotalMilliseconds}ms");
        }

        [Fact]
        public async Task Handle_CreatesPriceHistoryEntry() {
            // Arrange
            IPriceSentryDbContext _dbContextMemoery = CreateDbContext();
            var _handler = new CreateProductCommandHandler(
                _dbContextMemoery,
                _priceProviderMock.Object,
                _userManagerMock.Object);
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId, Email = "test@example.com" };
            var command = new CreateProductCommand {
                UserId = userId,
                DesiredPrice = 2000,
                ProductUrl = "https://example.com/pricehistory"
            };
            var currentPrice = 1999m;

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            _priceProviderMock.Setup(x => x.GetPriceAsync(command.ProductUrl, It.IsAny<CancellationToken>()))
                .ReturnsAsync(currentPrice);
            _priceProviderMock.Setup(x => x.GetTitleAsync(command.ProductUrl, It.IsAny<CancellationToken>()))
                .ReturnsAsync("Test");

            // Act
            var IdProduct = await _handler.Handle(command, CancellationToken.None);

            // Assert
            var result = await _dbContextMemoery.ProductPrices.FirstOrDefaultAsync(p => p.Price == currentPrice && p.ProductId != Guid.Empty, CancellationToken.None);
            Assert.NotNull(result);
            Assert.True(result.ProductId == IdProduct);

        }
    }
}