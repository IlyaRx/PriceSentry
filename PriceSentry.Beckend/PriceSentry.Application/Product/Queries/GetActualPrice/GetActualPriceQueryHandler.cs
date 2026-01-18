
using AutoMapper;
using MediatR;
using PriceSentry.Application.Common.Exceptions;
using PriceSentry.Application.Interfaces;
using PriceSentry.Domain;
using System.Data;

namespace PriceSentry.Application.Product.Queries.GetActualPrice {
    public class GetActualPriceQueryHandler :IRequestHandler<GetActualPriceQuery, ActualPriceVm>{
        private readonly IPriceSentryDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetActualPriceQueryHandler(IPriceSentryDbContext dbContext, IMapper mapper) => (_dbContext, _mapper) = (dbContext, mapper); 
        

        public async Task<ActualPriceVm> Handle(GetActualPriceQuery request, CancellationToken cancellationToken) {
            var product = await _dbContext.Products.FindAsync(new object[] {request.Id} ,cancellationToken);
            
            if (product == null || product.UserId != request.UserId) {
                throw new NotFoundException(nameof(TrackingProduct) , request);
            }

            return _mapper.Map<ActualPriceVm>(product);
        }
    }
}
