using FluentValidation.TestHelper;
using PriceSentry.Domain;
using PriceSentry.Application.Product.Queries.GetActualPrice;
using Xunit;

namespace PriceSentry.Tests.Validators;

public class GetActualPriceQueryValidatorTests {
    private readonly GetActualPriceQueryValidator _validator;

    public GetActualPriceQueryValidatorTests() {
        _validator = new GetActualPriceQueryValidator();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveError() {
        // Arrange
        var query = new GetActualPriceQuery {
            Id = Guid.Empty,
            UserId = Guid.NewGuid()
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenIdIsNotEmpty_ShouldNotHaveError() {
        // Arrange
        var query = new GetActualPriceQuery {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenUserIdIsEmpty_ShouldHaveError() {
        // Arrange
        var query = new GetActualPriceQuery {
            Id = Guid.NewGuid(),
            UserId = Guid.Empty
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WhenUserIdIsNotEmpty_ShouldNotHaveError() {
        // Arrange
        var query = new GetActualPriceQuery {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WhenBothIdsAreValid_ShouldNotHaveAnyErrors() {
        // Arrange
        var query = new GetActualPriceQuery {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}