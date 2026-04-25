using Microsoft.AspNetCore.Identity;
using Moq;
using PriceSentry.Application.Autorisation.Commands.Registration;
using PriceSentry.Application.Interfaces;
using PriceSentry.Application.Interfaces.Notifications;
using PriceSentry.Domain;
using Xunit;

namespace PriceSentry.Tests.Handlers {
    public class RegistrUserCommandHandlerTests {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IStoregCodeService> _storegMock;
        private readonly Mock<IGeneratedCode> _generatedCodeMock;
        private readonly Mock<IUserCodeNotificationService> _notificationMock;
        private readonly RegistrUserCommandHandler _handler;

        public RegistrUserCommandHandlerTests() {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
            _storegMock = new Mock<IStoregCodeService>();
            _generatedCodeMock = new Mock<IGeneratedCode>();
            _notificationMock = new Mock<IUserCodeNotificationService>();

            _handler = new RegistrUserCommandHandler(
                _storegMock.Object,
                _generatedCodeMock.Object,
                _notificationMock.Object,
                _userManagerMock.Object);
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotExist_CreatesNewUser() {
            // Arrange
            var email = "newuser@example.com";
            var command = new RegistrUserCommand { Email = email };
            var expectedCode = "ABC123";

            _userManagerMock.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync((ApplicationUser)null!);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>()))
                            .Callback<ApplicationUser>(user => user.Id = Guid.NewGuid())
                            .ReturnsAsync(IdentityResult.Success);
            _generatedCodeMock.Setup(x => x.GetCode()).Returns(expectedCode);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _userManagerMock.Verify(x => x.CreateAsync(It.Is<ApplicationUser>(u => u.Email == email && u.UserName == email)), Times.Once);
            Assert.NotEqual(Guid.Empty, result);
        }

        [Fact]
        public async Task Handle_WhenUserExists_DoesNotCreateDuplicate() {
            // Arrange
            var email = "existing@example.com";
            var existingUser = new ApplicationUser { Id = Guid.NewGuid(), Email = email };
            var command = new RegistrUserCommand { Email = email };
            var expectedCode = "XYZ789";

            _userManagerMock.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(existingUser);
            _generatedCodeMock.Setup(x => x.GetCode()).Returns(expectedCode);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>()), Times.Never);
            Assert.Equal(existingUser.Id, result);
        }

        [Fact]
        public async Task Handle_GeneratesAndStoresCode() {
            // Arrange
            var email = "test@example.com";
            var command = new RegistrUserCommand { Email = email };
            var expectedCode = "GEN456";

            _userManagerMock.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync((ApplicationUser)null!);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _generatedCodeMock.Setup(x => x.GetCode()).Returns(expectedCode);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _generatedCodeMock.Verify(x => x.GetCode(), Times.Once);
            _storegMock.Verify(x => x.StoreCodeAsync(email, expectedCode, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_SendsCodeNotification() {
            // Arrange
            var email = "notify@example.com";
            var command = new RegistrUserCommand { Email = email };
            var expectedCode = "NOT123";

            _ = _userManagerMock.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync((ApplicationUser)null!);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);
            _generatedCodeMock.Setup(x => x.GetCode()).Returns(expectedCode);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _notificationMock.Verify(x => x.SendCodeNotificationAsync(email, expectedCode), Times.Once);
        }
    }
}