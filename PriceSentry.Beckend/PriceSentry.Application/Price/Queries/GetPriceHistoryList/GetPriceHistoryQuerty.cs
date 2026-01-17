
using MediatR;

namespace PriceSentry.Application.Price.Queries.GetPriceHistoryList {
    public class GetPriceHistoryQuerty : IRequest<PriceListVm> {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
    }
}
