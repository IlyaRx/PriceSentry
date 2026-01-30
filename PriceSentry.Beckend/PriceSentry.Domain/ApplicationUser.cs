
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PriceSentry.Domain {
    public class ApplicationUser : IdentityUser<Guid>{
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        public override required string? Email { get; set; }

        public long? TelegramChatId { get; set;}
        public bool IsTelegramConfirmed { get; set; } = false;
        public virtual List<TrackingProduct> TrackingProducts { get; set; } = new List<TrackingProduct>();
    }
}
