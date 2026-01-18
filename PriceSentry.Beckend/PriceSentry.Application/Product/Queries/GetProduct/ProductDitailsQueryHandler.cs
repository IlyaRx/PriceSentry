
using AutoMapper;
using MediatR;
using PriceSentry.Application.Common.Exceptions;
using PriceSentry.Application.Interfaces;
using PriceSentry.Domain;

namespace PriceSentry.Application.Product.Queries.GetProduct {
    public class ProductDitailsQueryHandler : IRequestHandler<ProductDitailsQuery, ProductDitailsVm>{
        private readonly IPriceSentryDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProductDitailsQueryHandler(IPriceSentryDbContext dbContext, IMapper mapper) => (_dbContext, _mapper) 
                                                                                            = (dbContext, mapper);

        public async Task<ProductDitailsVm> Handle(ProductDitailsQuery request, CancellationToken cancellationToken) {
            var product = await _dbContext.Products.FindAsync(new object[] { request.Id }, cancellationToken);

            if (product == null || product.UserId != request.UserId) {
                throw new NotFoundException(nameof(TrackingProduct), request);
            }
            return _mapper.Map<ProductDitailsVm>(product);
        }
    }
}
