using FluentValidation.TestHelper;
using PriceSentry.Application.Price.Queries.GetPriceHistoryList;
using Xunit;

namespace PriceSentry.Tests.Validators;

public class GetPriceHistoryQueryValidatorTests {
    private readonly GetPriceHistoryQueryValidator _validator;

    public GetPriceHistoryQueryValidatorTests() {
        _validator = new GetPriceHistoryQueryValidator();
    }

    [Fact]
    public void Validate_WhenUserIdIsEmpty_ShouldHaveError() {
        // Arrange
        var query = new GetPriceHistoryQuery {
            UserId = Guid.Empty,
            ProductId = Guid.NewGuid()
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WhenUserIdIsNotEmpty_ShouldNotHaveError() {
        // Arrange
        var query = new GetPriceHistoryQuery {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid()
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WhenProductIdIsEmpty_ShouldHaveError() {
        // Arrange
        var query = new GetPriceHistoryQuery {
            UserId = Guid.NewGuid(),
            ProductId = Guid.Empty
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.ProductId);
    }

    [Fact]
    public void Validate_WhenProductIdIsNotEmpty_ShouldNotHaveError() {
        // Arrange
        var query = new GetPriceHistoryQuery {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid()
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductId);
    }

    [Fact]
    public void Validate_WhenBothIdsAreEmpty_ShouldHaveTwoErrors() {
        // Arrange
        var query = new GetPriceHistoryQuery {
            UserId = Guid.Empty,
            ProductId = Guid.Empty
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
        result.ShouldHaveValidationErrorFor(x => x.ProductId);
    }

    [Fact]
    public void Validate_WhenBothIdsAreValid_ShouldNotHaveAnyErrors() {
        // Arrange
        var query = new GetPriceHistoryQuery {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid()
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}