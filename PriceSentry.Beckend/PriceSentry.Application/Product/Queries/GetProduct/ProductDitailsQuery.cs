
using MediatR;

namespace PriceSentry.Application.Product.Queries.GetProduct {
    public class ProductDitailsQuery : IRequest<ProductDitailsVm>{
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
    }
}
