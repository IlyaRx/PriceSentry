
using PriceSentry.Domain;

namespace PriceSentry.Application.Interfaces {
    public interface IPriceDropChecker {
        public bool ShouldNotify(TrackingProduct product, decimal newPrice);
    }
}
