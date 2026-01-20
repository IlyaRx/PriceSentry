using PriceSentry.Application.Interfaces;
using PriceSentry.Domain;

namespace PriceSentry.Application.Sepvices {
    public class PriceDropCheckerService : IPriceDropChecker {
        public bool ShouldNotify(TrackingProduct product, decimal newPrice) {
            return newPrice <= product.DesiredPrice;
        }
    }
}
