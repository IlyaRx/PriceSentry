
using PriceSentry.Domain;

namespace PriceSentry.Application.Interfaces {
    public interface INotificationService {
        public Task SendPriceDropNotificationAsync(TrackingProduct product, decimal oldPrice, decimal newPrice);
    }
}
