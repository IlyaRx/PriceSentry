
using PriceSentry.Application.Interfaces;

namespace PriceSentry.Persistence.Providers {
    public class SystemTimeProvider : ITimeProvider {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
