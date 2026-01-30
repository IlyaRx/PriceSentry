
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PriceSentry.Application.Interfaces;

namespace PriceSentry.Persistence.Services {
    public class TrackingBackgroundService : BackgroundService {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromHours(24);
        public TrackingBackgroundService(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
            while (!cancellationToken.IsCancellationRequested) {
                try {
                    using var scope = _serviceProvider.CreateScope();

                    var trackingService = scope.ServiceProvider.GetRequiredService<ITrackingService>();

                    await trackingService.TrackAllProductsAsync(cancellationToken);

                    await Task.Delay(_interval, cancellationToken);

                } catch (Exception ex) { throw ex; }
            }
        }
    }
}
