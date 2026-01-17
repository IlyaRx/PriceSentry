using MediatR;
using PriceSentry.Application.Interfaces.cs;
using PriceSentry.Domain;

namespace PriceSentry.Application.Product.Commands.Create {
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid> {
        private readonly IPriceSentryDbContext _dbContext;
        private readonly IPriceParserService _priceParserService;

        public CreateProductCommandHandler(IPriceSentryDbContext dbContext, IPriceParserService priceParserService) => 
                                          (_dbContext, _priceParserService) = (dbContext, priceParserService);
        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken) {

            var priceTask = _priceParserService.GetPriceAsync(request.ProductUrl, cancellationToken);
            var titleTask = _priceParserService.GetTitleAsync(request.ProductUrl, cancellationToken);
            await Task.WhenAll(priceTask, titleTask);

            var product = new TrackingProduct {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ProductUrl = request.ProductUrl,
                DesiredPrice = request.DesiredPrice,
                ActualPrice = await priceTask,
                Title = await titleTask,
                LastTracking = DateTime.Now,
            };
            var price = new ProductPriceHistory {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                AddDate = DateTime.UtcNow,
                Price = product.ActualPrice,
                TrackingProduct = product,
            };
            

            await _dbContext.Products.AddAsync(product, cancellationToken);
            await _dbContext.ProductPrices.AddAsync(price, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return product.Id;
        }
    }
}
