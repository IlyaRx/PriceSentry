using FluentValidation.TestHelper;
using PriceSentry.Application.Product.Queries.GetProduct;
using Xunit;

namespace PriceSentry.Tests.Validators;

public class ProductDitailsQueryValidatorTests {
    private readonly ProductDitailsQueryValidator _validator;

    public ProductDitailsQueryValidatorTests() {
        _validator = new ProductDitailsQueryValidator();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveError() {
        // Arrange
        var query = new ProductDitailsQuery {
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
        var query = new ProductDitailsQuery {
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
        var query = new ProductDitailsQuery {
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
        var query = new ProductDitailsQuery {
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
        var query = new ProductDitailsQuery {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}