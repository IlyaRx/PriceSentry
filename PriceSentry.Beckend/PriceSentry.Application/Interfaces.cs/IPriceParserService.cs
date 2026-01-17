

namespace PriceSentry.Application.Interfaces.cs {
    public interface IPriceParserService {
        Task<decimal> GetPriceAsync(string url, CancellationToken cancellationToken);
        Task<string> GetTitleAsync(string url, CancellationToken cancellationToken);
    }
}
