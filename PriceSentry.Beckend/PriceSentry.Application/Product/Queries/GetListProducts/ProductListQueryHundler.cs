
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PriceSentry.Application.Common.Exceptions;
using PriceSentry.Application.Interfaces.cs;
using PriceSentry.Domain;

namespace PriceSentry.Application.Product.Queries.GetListProducts {
    public class ProductListQueryHundler : IRequestHandler<ProductListQuery, ProductListVm> {
        private readonly IPriceSentryDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProductListQueryHundler(IPriceSentryDbContext dbContext, IMapper mapper) {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ProductListVm> Handle(ProductListQuery request, CancellationToken cancellationToken) {
            var products = await _dbContext.Products
                            .Where(p => p.UserId == request.UserId)
                            .ProjectTo<ProductLookupVm>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

            return new ProductListVm { ProductList = products};
        }
    }
}
