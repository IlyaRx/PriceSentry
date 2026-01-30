using PriceSentry.Domain;

namespace PriceSentry.Application.Interfaces.Notifications {
    public interface IPriceNotificationService { // сервис оповещения
        public Task SendPriceDropNotificationAsync(TrackingProduct product, decimal newPrice);
    }
}
