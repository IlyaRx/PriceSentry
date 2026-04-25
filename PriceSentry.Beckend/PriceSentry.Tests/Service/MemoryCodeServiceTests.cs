using PriceSentry.Persistence.Providers;
using PriceSentry.Persistence.Services;
using Xunit;

namespace PriceSentry.Tests.Service {
    public class MemoryCodeServiceTests {
        private readonly MemoryCodeService _service;
        private readonly CancellationToken _cancellationToken;

        public MemoryCodeServiceTests() {
            _service = new MemoryCodeService(new TestTimeProvider());
            _cancellationToken = CancellationToken.None;
        }

        [Fact]
        public async Task StoreCodeAsync_ShouldStoreCode_WhenValidInput() {
            // Arrange
            var key = "user@example.com";
            var code = "ABC123";

            // Act
            await _service.StoreCodeAsync(key, code, _cancellationToken);
            var isValid = await _service.IsValidCodeAsync(key, code, _cancellationToken);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public async Task StoreCodeAsync_ShouldOverwriteExistingKey_WhenCalledAgain() {
            // Arrange
            var key = "user@example.com";
            var firstCode = "ABC123";
            var overwritingCode = "EFG321";

            // Act
            await _service.StoreCodeAsync(key, firstCode, _cancellationToken);
            await _service.StoreCodeAsync(key, overwritingCode, _cancellationToken);

            var isValidFirst = await _service.IsValidCodeAsync(key, firstCode, _cancellationToken);
            var isValidOverwriting = await _service.IsValidCodeAsync(key, overwritingCode, _cancellationToken);

            // Assert
            Assert.False(isValidFirst);
            Assert.True(isValidOverwriting);
        }

        [Fact]
        public async Task StoreCodeAsync_ShouldStoreWithFiveAttempts() {
            // Arrange & Act
            await _service.StoreCodeAsync("key", "CODE", _cancellationToken);

            for (int i = 0; i < 4; i++) {
                var wrongResult = await _service.IsValidCodeAsync("key", "WRONG", _cancellationToken);
                Assert.False(wrongResult);
            }

            var correctResult = await _service.IsValidCodeAsync("key", "CODE", _cancellationToken);
            Assert.True(correctResult);
            var correctResultAfterFifthAttempt = await _service.IsValidCodeAsync("key", "CODE", _cancellationToken);
            Assert.False(correctResultAfterFifthAttempt);

        }


        [Fact]
        public async Task IsValidCodeAsync_ShouldReturnTrue_WhenCorrectCodeAndNotExpired() {
            // Arrange
            var key = "user@example.com";
            var code = "VALID123";
            await _service.StoreCodeAsync(key, code, _cancellationToken);

            // Act
            var result = await _service.IsValidCodeAsync(key, code, _cancellationToken);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsValidCodeAsync_ShouldReturnFalse_WhenWrongCode() {
            // Arrange
            await _service.StoreCodeAsync("user@example.com", "CORRECT", _cancellationToken);

            // Act
            var result = await _service.IsValidCodeAsync("user@example.com", "WRONG", _cancellationToken);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsValidCodeAsync_ShouldReturnFalse_WhenKeyDoesNotExist() {
            // Act
            var result = await _service.IsValidCodeAsync("nonexistent@example.com", "ANY", _cancellationToken);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsValidCodeAsync_ShouldDecrementAttempts_WhenWrongCodeProvided() {
            // Arrange
            var key = "user@example.com";
            await _service.StoreCodeAsync(key, "SECRET", _cancellationToken);

            // Act
            for (int i = 0; i < 5; i++) {
                await _service.IsValidCodeAsync(key, "WRONG", _cancellationToken);
            }

            // Assert
            var isValid = await _service.IsValidCodeAsync(key, "SECRET", _cancellationToken);
            Assert.False(isValid);
        }

        [Fact]
        public async Task IsValidCodeAsync_ShouldReturnFalse_WhenAttemptsExhausted() {
            // Arrange
            var key = "user@example.com";
            await _service.StoreCodeAsync(key, "SECRET", _cancellationToken);

            // Act
            for (int i = 0; i < 5; i++) {
                await _service.IsValidCodeAsync(key, "WRONG", _cancellationToken);
            }

            // Assert
            var isValid = await _service.IsValidCodeAsync(key, "SECRET", _cancellationToken);
            Assert.False(isValid);
        }

        [Fact]
        public async Task IsValidCodeAsync_ShouldReturnFalse_WhenCodeExpired() {
            // Arrange
            var timeProvider = new TestTimeProvider { UtcNow = DateTime.UtcNow};
            var service = new MemoryCodeService(timeProvider);

            // Act
            await service.StoreCodeAsync("key", "code", CancellationToken.None);
            timeProvider.UtcNow = DateTime.UtcNow.AddDays(1);
            var result = await service.IsValidCodeAsync("key", "code", CancellationToken.None);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsValidCodeAsync_ShouldRemoveKey_WhenValidCodeEntered() {
            // Arrange
            var key = "user@example.com";
            var code = "ONETIME";
            await _service.StoreCodeAsync(key, code, _cancellationToken);

            // Act
            var isValid = await _service.IsValidCodeAsync(key, code, _cancellationToken);

            // Assert
            Assert.True(isValid);
            var secondTry = await _service.IsValidCodeAsync(key, code, _cancellationToken);
            Assert.False(secondTry);
        }

        [Fact]
        public async Task IsValidCodeAsync_ShouldHandleEmptyKey() {
            // Act
            var result = await _service.IsValidCodeAsync("", "ANY", _cancellationToken);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsValidCodeAsync_ShouldHandleNullCode() {
            // Arrange
            await _service.StoreCodeAsync("key", "CODE", _cancellationToken);

            // Act 

            var result = await _service.IsValidCodeAsync("key", null, _cancellationToken);

            //Assert
            Assert.False(result);

        }

        [Fact]
        public async Task RemoveCodeAsync_ShouldDeleteExistingKey() {
            // Arrange
            var key = "user@example.com";
            await _service.StoreCodeAsync(key, "CODE", _cancellationToken);

            // Act
            await _service.RemoveCodeAsync(key, _cancellationToken);

            // Assert
            var isValid = await _service.IsValidCodeAsync(key, "CODE", _cancellationToken);
            Assert.False(isValid);
        }

        [Fact]
        public async Task RemoveCodeAsync_ShouldNotThrow_WhenKeyDoesNotExist() {
            // Act & Assert
            var exception = await Record.ExceptionAsync(() =>
                _service.RemoveCodeAsync("nonexistent", _cancellationToken));

            Assert.Null(exception);
        }        
    }
}
