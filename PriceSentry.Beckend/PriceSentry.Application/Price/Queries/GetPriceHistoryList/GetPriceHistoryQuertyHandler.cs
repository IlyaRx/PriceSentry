
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PriceSentry.Application.Interfaces.cs;

namespace PriceSentry.Application.Price.Queries.GetPriceHistoryList {
    public class GetPriceHistoryQuertyHandler :IRequestHandler<GetPriceHistoryQuerty, PriceListVm> {
        private readonly IPriceSentryDbContext _dbContext;
        private readonly IMapper _mapper;

        public GetPriceHistoryQuertyHandler(IMapper mapper, IPriceSentryDbContext dbContext) => (_mapper, _dbContext)
                                                                                              = (mapper, dbContext);

        public async Task<PriceListVm> Handle(GetPriceHistoryQuerty request, CancellationToken cancellationToken) {
            var prices = await _dbContext.ProductPrices
                        .Where(p => p.ProductId == request.ProductId)
                        .ProjectTo<PriceLookupDTO>(_mapper.ConfigurationProvider)
                        .ToListAsync();

            return new PriceListVm { Prices = prices };
        }
    }
}
