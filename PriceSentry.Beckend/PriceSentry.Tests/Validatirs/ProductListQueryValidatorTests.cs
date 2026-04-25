using FluentValidation.TestHelper;
using PriceSentry.Application.Product.Queries.GetListProducts;
using Xunit;

namespace PriceSentry.Tests.Validators;

public class ProductListQueryValidatorTests {
    private readonly ProductListQueryValidator _validator;

    public ProductListQueryValidatorTests() {
        _validator = new ProductListQueryValidator();
    }

    [Fact]
    public void Validate_WhenUserIdIsEmpty_ShouldHaveError() {
        // Arrange
        var query = new ProductListQuery { UserId = Guid.Empty };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WhenUserIdIsNotEmpty_ShouldNotHaveError() {
        // Arrange
        var query = new ProductListQuery { UserId = Guid.NewGuid() };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WhenUserIdIsValid_ShouldNotHaveAnyErrors() {
        // Arrange
        var query = new ProductListQuery { UserId = Guid.NewGuid() };

        // Act & Assert
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}