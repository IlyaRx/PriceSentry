
using PriceSentry.Application.Interfaces;

namespace PriceSentry.Persistence.Providers {
    public class TestTimeProvider : ITimeProvider {
        public DateTime UtcNow { get; set; } = DateTime.UtcNow;
    }
}
