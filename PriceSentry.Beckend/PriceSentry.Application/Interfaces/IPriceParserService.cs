

namespace PriceSentry.Application.Interfaces {
    public interface IPriceParserService {
        Task<decimal> GetPriceAsync(string url, CancellationToken cancellationToken);
        Task<string> GetTitleAsync(string url, CancellationToken cancellationToken);
    }
}
