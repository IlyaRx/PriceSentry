
namespace PriceSentry.Persistence.Interfases {
    public interface IShopPriceParser {
        bool CanParse(string url);
        Task<decimal> ParsePriceAsync(string url, CancellationToken cancellationToken);
        Task<string> ParseTitleAsync(string url, CancellationToken cancellationToken);
        Task<string> FetchHtmlAsync(string url, CancellationToken cancellationToken);
    }
}
