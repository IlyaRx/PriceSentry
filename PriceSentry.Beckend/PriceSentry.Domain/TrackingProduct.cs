

namespace PriceSentry.Domain {
    public class TrackingProduct {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required decimal DesiredPrice { get; set; }
        public required string ProductUrl { get; set; }
        public decimal ActualPrice {  get; set; }
        public string? Title { get; set; }
        public DateTime? LastTracking { get; set; }

        public List<ProductPriceHistory> PriceHistory { get; set; } = new List<ProductPriceHistory>();
    }
}
