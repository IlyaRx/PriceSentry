using FluentValidation.TestHelper;
using PriceSentry.Application.Product.Commands.Update;
using Xunit;

namespace PriceSentry.Tests.Validators;

public class UpdateProductCommandValidatorTests {
    private readonly UpdateProductCommandValidator _validator;

    public UpdateProductCommandValidatorTests() {
        _validator = new UpdateProductCommandValidator();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveError() {
        // Arrange
        var command = new UpdateProductCommand {
            Id = Guid.Empty,
            UserId = Guid.NewGuid(),
            DesiredPrice = 1000
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenIdIsNotEmpty_ShouldNotHaveError() {
        // Arrange
        var command = new UpdateProductCommand {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            DesiredPrice = 1000
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenUserIdIsEmpty_ShouldHaveError() {
        // Arrange
        var command = new UpdateProductCommand {
            Id = Guid.NewGuid(),
            UserId = Guid.Empty,
            DesiredPrice = 1000
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WhenUserIdIsNotEmpty_ShouldNotHaveError() {
        // Arrange
        var command = new UpdateProductCommand {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            DesiredPrice = 1000
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.UserId);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(1000001)]
    [InlineData(2000000)]
    public void Validate_WhenDesiredPriceOutOfRange_ShouldHaveError(decimal invalidPrice) {
        // Arrange
        var command = new UpdateProductCommand {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            DesiredPrice = invalidPrice
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DesiredPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(500000)]
    [InlineData(1000000)]
    public void Validate_WhenDesiredPriceInRange_ShouldNotHaveError(decimal validPrice) {
        // Arrange
        var command = new UpdateProductCommand {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            DesiredPrice = validPrice
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.DesiredPrice);
    }

    [Fact]
    public void Validate_WhenAllFieldsValid_ShouldNotHaveAnyErrors() {
        // Arrange
        var command = new UpdateProductCommand {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            DesiredPrice = 5000
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}