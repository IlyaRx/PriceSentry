
using FluentValidation;

namespace PriceSentry.Application.Price.Queries.GetPriceHistoryList {
    public class GetPriceHistoryQueryValidator : AbstractValidator<GetPriceHistoryQuery> {
        public GetPriceHistoryQueryValidator() {
            RuleFor(getPriceHistoryQuery => getPriceHistoryQuery.UserId).NotEqual(Guid.Empty);
            RuleFor(getPriceHistoryQuery => getPriceHistoryQuery.ProductId).NotEqual(Guid.Empty);
        }
    }
}
