using MediatR;
using PriceSentry.Application.Product.Queries.GetActualPrice;

namespace PriceSentry.Domain {
    public class GetActualPriceQuery : IRequest<ActualPriceVm>{
        public Guid UserId { get; set; }  
        public Guid Id { get; set; }
    }
}
