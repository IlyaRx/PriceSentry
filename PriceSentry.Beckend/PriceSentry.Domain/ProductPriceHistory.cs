

namespace PriceSentry.Domain {
    public class ProductPriceHistory {
        public Guid Id { get; set; }
        public Guid TrackingProductId { get; set; }
        public bool IsActive { get; set; }
        public decimal Price { get; set; }
        public DateTime AddDate { get; set; }

        public required TrackingProduct TrackingProduct { get; set; }
    }
}
