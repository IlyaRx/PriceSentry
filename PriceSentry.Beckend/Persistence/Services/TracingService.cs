
using Microsoft.EntityFrameworkCore;
using PriceSentry.Application.Interfaces;
using PriceSentry.Application.Interfaces.Notifications;
using PriceSentry.Domain;

namespace PriceSentry.Persistence.Services {
    public class TracingService : ITrackingService {
        private readonly IPriceSentryDbContext _dbContext;
        private readonly IProductPriceProvider _priceParserService;
        private readonly IPriceDropChecker _priceDropChecker;
        private readonly IEnumerable<IPriceNotificationService> _notificationService;

        public TracingService(IPriceSentryDbContext dbContext, 
                              IProductPriceProvider priceParserService, 
                              IPriceDropChecker priceDropChecker,
                              IEnumerable<IPriceNotificationService> notificationService) {
            _dbContext = dbContext;
            _priceParserService = priceParserService;
            _priceDropChecker = priceDropChecker;
            _notificationService = notificationService;
        }

        public async Task TrackAllProductsAsync(CancellationToken cancellationToken) {
            var products = await _dbContext.Products.ToListAsync(cancellationToken);

            foreach (var product in products) {
                var currentPrice = await _priceParserService.GetPriceAsync(product.ProductUrl, cancellationToken);

                if(_priceDropChecker.ShouldNotify(product, currentPrice)) {
                    foreach (var notification in _notificationService) {
                        await notification.SendPriceDropNotificationAsync(product, currentPrice);
                    }
                }

                product.ActualPrice = currentPrice;
                product .LastTracking = DateTime.UtcNow;
                await _dbContext.ProductPrices.AddAsync(new ProductPriceHistory() {
                    TrackingProduct = product,
                    Id = Guid.NewGuid(),
                    Price = currentPrice,
                    AddDate = DateTime.UtcNow,
                    ProductId = product.Id
                }, cancellationToken);
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
