using FluentValidation.TestHelper;
using PriceSentry.Application.Product.Commands.Create;
using Xunit;

namespace PriceSentry.Tests.Validators;

public class CreateProductCommandValidatorTests {
    private readonly CreateProductCommandValidator _validator;

    public CreateProductCommandValidatorTests() {
        _validator = new CreateProductCommandValidator();
    }

    [Fact]
    public void Validate_WhenUserIdIsEmpty_ShouldHaveError() {
        // Arrange
        var command = new CreateProductCommand {
            UserId = Guid.Empty,
            DesiredPrice = 1000,
            ProductUrl = "https://example.com/product"
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WhenUserIdIsNotEmpty_ShouldNotHaveError() {
        // Arrange
        var command = new CreateProductCommand {
            UserId = Guid.NewGuid(),
            DesiredPrice = 1000,
            ProductUrl = "https://example.com/product"
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
        var command = new CreateProductCommand {
            UserId = Guid.NewGuid(),
            DesiredPrice = invalidPrice,
            ProductUrl = "https://example.com/product"
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
        var command = new CreateProductCommand {
            UserId = Guid.NewGuid(),
            DesiredPrice = validPrice,
            ProductUrl = "https://example.com/product"
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.DesiredPrice);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WhenProductUrlIsEmpty_ShouldHaveError(string? invalidUrl) {
        // Arrange
        var command = new CreateProductCommand {
            UserId = Guid.NewGuid(),
            DesiredPrice = 1000,
            ProductUrl = invalidUrl!
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ProductUrl);
    }

    [Fact]
    public void Validate_WhenProductUrlIsProvided_ShouldNotHaveError() {
        // Arrange
        var command = new CreateProductCommand {
            UserId = Guid.NewGuid(),
            DesiredPrice = 1000,
            ProductUrl = "https://citilink.ru/product/123456"
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductUrl);
    }

    [Fact]
    public void Validate_WhenAllFieldsValid_ShouldNotHaveAnyErrors() {
        // Arrange
        var command = new CreateProductCommand {
            UserId = Guid.NewGuid(),
            DesiredPrice = 5000,
            ProductUrl = "https://example.com/product"
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}