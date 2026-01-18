
using FluentValidation;
using PriceSentry.Domain;

namespace PriceSentry.Application.Product.Queries.GetActualPrice {
    public class GetActualPriceQueryValidator : AbstractValidator<GetActualPriceQuery> {
        public GetActualPriceQueryValidator() {
            RuleFor(getActualPriceQuery => getActualPriceQuery.Id).NotEqual(Guid.Empty);
            RuleFor(getActualPriceQuery => getActualPriceQuery.UserId).NotEqual(Guid.Empty);
        }
    }
}
