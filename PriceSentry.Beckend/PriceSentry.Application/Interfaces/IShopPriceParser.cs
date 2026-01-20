
namespace PriceSentry.Persistence.Interfases {
    public interface IShopPriceParser {
        bool CanParse(string url);
        Task<decimal> ParsePriceAsync(string html, CancellationToken cancellationToken);
        Task<string> ParseTtileAsync(string html, CancellationToken cancellationToken);
    }
}
