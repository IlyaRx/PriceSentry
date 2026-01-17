

namespace PriceSentry.Domain {
    public class ProductPriceHistory {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public decimal Price { get; set; }
        public DateTime AddDate { get; set; }

        public virtual required TrackingProduct TrackingProduct { get; set; }
    }
}
