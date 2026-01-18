
using MediatR;

namespace PriceSentry.Application.Price.Queries.GetPriceHistoryList {
    public class GetPriceHistoryQuery : IRequest<PriceListVm> {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
    }
}
