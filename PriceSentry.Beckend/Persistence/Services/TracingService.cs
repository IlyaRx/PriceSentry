
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PriceSentry.Application.Common.Exceptions;
using PriceSentry.Application.Interfaces;
using PriceSentry.Application.Interfaces.Notifications;
using PriceSentry.Domain;

namespace PriceSentry.Persistence.Services {
    public class TracingService : ITrackingService {
        private readonly IPriceSentryDbContext _dbContext;
        private readonly IProductPriceProvider _priceParserService;
        private readonly IPriceDropChecker _priceDropChecker;
        private readonly IEnumerable<IPriceNotificationService> _notificationService;
        private readonly ILogger<TracingService> _logger;

        public TracingService(IPriceSentryDbContext dbContext,
                              IProductPriceProvider priceParserService,
                              IPriceDropChecker priceDropChecker,
                              IEnumerable<IPriceNotificationService> notificationService,
                              ILogger<TracingService> logger) {
            _dbContext = dbContext;
            _priceParserService = priceParserService;
            _priceDropChecker = priceDropChecker;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task TrackAllProductsAsync(CancellationToken cancellationToken) {
            var products = await _dbContext.Products.ToListAsync(cancellationToken);

            _logger.LogInformation($"Начинаю проверку {products.Count} товаров");

            foreach (var product in products) {
                try {
                    var currentPrice = await _priceParserService.GetPriceAsync(product.ProductUrl, cancellationToken);

                    _logger.LogInformation($"Ищу соответствии усовий оповещения {product.Title} - {currentPrice}₽, ответ {_priceDropChecker.ShouldNotify(product, currentPrice)}");
                    if (_priceDropChecker.ShouldNotify(product, currentPrice)) {
                        _logger.LogInformation($"Найдено соответствии усовий оповещения {product.Title} - {currentPrice}₽");
                        foreach (var notification in _notificationService) {
                            await notification.SendPriceDropNotificationAsync(product, currentPrice);
                        }
                    }

                    product.ActualPrice = currentPrice;
                    product.LastTracking = DateTime.UtcNow;
                    await _dbContext.ProductPrices.AddAsync(new ProductPriceHistory() {
                        TrackingProduct = product,
                        Id = Guid.NewGuid(),
                        Price = currentPrice,
                        AddDate = DateTime.UtcNow,
                        ProductId = product.Id
                    }, cancellationToken);
                } catch (RateLimitException ex) {
                    _logger.LogWarning("Превышен лимит запросов для {ProductUrl}: {Message}", product.ProductUrl, ex.Message);
                } catch (Exception ex) {
                    _logger.LogError(ex, $"\nОшибка при проверки {product.ProductUrl}");
                }
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Проверка завершена");

        }
    }
}
