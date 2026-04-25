using FluentValidation.TestHelper;
using PriceSentry.Application.Autorisation.Commands.Registration;
using Xunit;

namespace PriceSentry.Tests.Validators;

public class RegistrUserCommandValidatorTests {
    private readonly RegistrUserCommandValidator _validator;

    public RegistrUserCommandValidatorTests() {
        _validator = new RegistrUserCommandValidator();
    }

    [Fact]
    public void Validate_WhenEmailIsEmpty_ShouldHaveError() {
        // Arrange
        var command = new RegistrUserCommand { Email = "" };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WhenEmailIsNull_ShouldHaveError() {
        // Arrange
        var command = new RegistrUserCommand { Email = null! };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("missing@domain")]
    [InlineData("user@.com")]
    [InlineData("user@domain.")]
    [InlineData("user@domain..com")]
    [InlineData("user@domain.......com")]
    public void Validate_WhenEmailFormatIsInvalid_ShouldHaveError(string invalidEmail) {
        // Arrange
        var command = new RegistrUserCommand { Email = invalidEmail };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test.user@domain.co.uk")]
    [InlineData("name+tag@example.org")]
    public void Validate_WhenEmailIsValid_ShouldNotHaveError(string validEmail) {
        // Arrange
        var command = new RegistrUserCommand { Email = validEmail };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }
}