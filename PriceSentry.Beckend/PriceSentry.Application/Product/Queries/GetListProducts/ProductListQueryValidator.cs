
using FluentValidation;

namespace PriceSentry.Application.Product.Queries.GetListProducts {
    public class ProductListQueryValidator : AbstractValidator<ProductListQuery> {
        public ProductListQueryValidator() { 
            RuleFor(productListQuery => productListQuery.UserId).NotEqual(Guid.Empty);
        }
    }
}
