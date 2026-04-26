using MediatR;
using Microsoft.AspNetCore.Identity;
using Moq;
using PriceSentry.Application.Autorisation.Commands.Verification;
using PriceSentry.Application.Common.Exceptions;
using PriceSentry.Application.Interfaces;
using PriceSentry.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PriceSentry.Tests.Handlers {
    public class VerificationUserCommandHandlerTests {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IStoregCodeService> _storegeMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly VerificationUserCommandHandler _handler;

        public VerificationUserCommandHandlerTests() {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
            _storegeMock = new Mock<IStoregCodeService>();
            _tokenServiceMock = new Mock<ITokenService>();

            _handler = new VerificationUserCommandHandler(
                _storegeMock.Object,
                _userManagerMock.Object,
                _tokenServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCode_ReturnsToken() {
            // Arrange
            var email = "valid@example.com";
            var code = "CORRECT";
            var expectedToken = "jwt_token_here";
            var user = new ApplicationUser { Id = Guid.NewGuid(), Email = email };

            var command = new VerificationUserCommand { Email = email, Code = code };

            _userManagerMock.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(user);
            _storegeMock.Setup(x => x.IsValidCodeAsync(email, code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _tokenServiceMock.Setup(x => x.GenerateTokenAsync(user, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedToken);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(expectedToken, result);
            _storegeMock.Verify(x => x.RemoveCodeAsync(email, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserNotFound_ThrowsNotFoundException() {
            // Arrange
            var email = "notfound@example.com";
            var command = new VerificationUserCommand { Email = email, Code = "123456" };

            _userManagerMock.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync((ApplicationUser)null!);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithInvalidCode_ThrowsInvalidCodeException() {
            // Arrange
            var email = "invalid@example.com";
            var code = "WRONG";
            var user = new ApplicationUser { Id = Guid.NewGuid(), Email = email };
            var command = new VerificationUserCommand { Email = email, Code = code };

            _userManagerMock.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(user);
            _storegeMock.Setup(x => x.IsValidCodeAsync(email, code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCodeException>(
                () => _handler.Handle(command, CancellationToken.None));

            _storegeMock.Verify(x => x.RemoveCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenCodeIsValid_RemovesCodeFromStorage() {
            // Arrange
            var email = "remove@example.com";
            var code = "REMOVE";
            var user = new ApplicationUser { Id = Guid.NewGuid(), Email = email };
            var command = new VerificationUserCommand { Email = email, Code = code };

            _userManagerMock.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(user);
            _storegeMock.Setup(x => x.IsValidCodeAsync(email, code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _tokenServiceMock.Setup(x => x.GenerateTokenAsync(user, It.IsAny<CancellationToken>()))
                .ReturnsAsync("token");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _storegeMock.Verify(x => x.RemoveCodeAsync(email, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}