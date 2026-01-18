
using FluentValidation;

namespace PriceSentry.Application.Product.Queries.GetProduct {
    public class ProductDitailsQueryValidator : AbstractValidator<ProductDitailsQuery> {
        public ProductDitailsQueryValidator() { 
            RuleFor(productDitailsQuery => productDitailsQuery.Id).NotEqual(Guid.Empty);
            RuleFor(productDitailsQuery => productDitailsQuery.UserId).NotEqual(Guid.Empty);
        }
    }
}
