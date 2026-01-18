
namespace PriceSentry.Application.Interfaces {
    public interface ITrackingService {
        Task TrackAllProductsAsync(CancellationToken cancellationToken);
    }
}
