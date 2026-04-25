using FluentValidation.TestHelper;
using PriceSentry.Application.Autorisation.Commands.Verification;
using Xunit;

namespace PriceSentry.Tests.Validators;

public class VerificationUserCommandValidatorTests {
    private readonly VerificationUserCommandValidator _validator;

    public VerificationUserCommandValidatorTests() {
        _validator = new VerificationUserCommandValidator();
    }

    [Fact]
    public void Validate_WhenEmailIsEmpty_ShouldHaveError() {
        // Arrange
        var command = new VerificationUserCommand { Email = "", Code = "123456" };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WhenEmailIsNull_ShouldHaveError() {
        // Arrange
        var command = new VerificationUserCommand { Email = null!, Code = "123456" };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("missing@domain")]
    public void Validate_WhenEmailFormatIsInvalid_ShouldHaveError(string invalidEmail) {
        // Arrange
        var command = new VerificationUserCommand { Email = invalidEmail, Code = "123456" };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WhenCodeIsEmpty_ShouldHaveError() {
        // Arrange
        var command = new VerificationUserCommand { Email = "test@example.com", Code = "" };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Validate_WhenCodeIsNull_ShouldHaveError() {
        // Arrange
        var command = new VerificationUserCommand { Email = "test@example.com", Code = null! };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Validate_WhenAllFieldsValid_ShouldNotHaveAnyErrors() {
        // Arrange
        var command = new VerificationUserCommand {
            Email = "test@example.com",
            Code = "ABC123"
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}