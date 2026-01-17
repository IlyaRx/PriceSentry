
using MediatR;

namespace PriceSentry.Application.Product.Queries.GetListProducts {
    public class ProductListQuery : IRequest<ProductListVm>{
        public Guid UserId { get; set; }
    }
}
