
namespace PriceSentry.Domain {
    public class ApplicationUser {
        public Guid Id { get; set; }
        public long? TelegramChatId { get; set;}
        public required string Email { get; set; }

        public virtual List<TrackingProduct> TrackingProducts { get; set; } = new List<TrackingProduct>();
    }
}
