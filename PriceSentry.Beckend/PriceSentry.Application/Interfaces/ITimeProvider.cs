
namespace PriceSentry.Application.Interfaces {
    public interface ITimeProvider {
        DateTime UtcNow { get; }
    }
}
