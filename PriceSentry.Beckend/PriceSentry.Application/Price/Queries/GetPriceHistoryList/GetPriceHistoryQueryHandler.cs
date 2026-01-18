
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PriceSentry.Application.Interfaces;

namespace PriceSentry.Application.Price.Queries.GetPriceHistoryList {
    public class GetPriceHistoryQueryHandler :IRequestHandler<GetPriceHistoryQuery, PriceListVm> {
        private readonly IPriceSentryDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetPriceHistoryQueryHandler(IMapper mapper, IPriceSentryDbContext dbContext) => (_mapper, _dbContext)
                                                                                              = (mapper, dbContext);

        public async Task<PriceListVm> Handle(GetPriceHistoryQuery request, CancellationToken cancellationToken) {
            var prices = await _dbContext.ProductPrices
                        .Where(p => p.ProductId == request.ProductId)
                        .ProjectTo<PriceLookupDTO>(_mapper.ConfigurationProvider)
                        .ToListAsync();

            return new PriceListVm { Prices = prices };
        }
    }
}
