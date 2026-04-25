using FluentValidation.TestHelper;
using PriceSentry.Application.Product.Commands.Delete;
using Xunit;

namespace PriceSentry.Tests.Validators;

public class DeleteProductCommandValidatorTests {
    private readonly DeleteProductCommandValidator _validator;

    public DeleteProductCommandValidatorTests() {
        _validator = new DeleteProductCommandValidator();
    }

    [Fact]
    public void Validate_WhenUserIdIsEmpty_ShouldHaveError() {
        // Arrange
        var command = new DeleteProductCommand {
            UserId = Guid.Empty,
            Id = Guid.NewGuid()
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WhenUserIdIsNotEmpty_ShouldNotHaveError() {
        // Arrange
        var command = new DeleteProductCommand {
            UserId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WhenProductIdIsEmpty_ShouldHaveError() {
        // Arrange
        var command = new DeleteProductCommand {
            UserId = Guid.NewGuid(),
            Id = Guid.Empty
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenProductIdIsNotEmpty_ShouldNotHaveError() {
        // Arrange
        var command = new DeleteProductCommand {
            UserId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenBothIdsAreEmpty_ShouldHaveTwoErrors() {
        // Arrange
        var command = new DeleteProductCommand {
            UserId = Guid.Empty,
            Id = Guid.Empty
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenBothIdsAreValid_ShouldNotHaveAnyErrors() {
        // Arrange
        var command = new DeleteProductCommand {
            UserId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        };

        // Act & Assert
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DifferentUserIds_ShouldValidateCorrectly() {
        // Arrange
        var command1 = new DeleteProductCommand {
            UserId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        };

        var command2 = new DeleteProductCommand {
            UserId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        };

        // Act & Assert
        var result1 = _validator.TestValidate(command1);
        var result2 = _validator.TestValidate(command2);

        Assert.True(result1.IsValid);
        Assert.True(result2.IsValid);
    }
}