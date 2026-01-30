

namespace PriceSentry.Application.Interfaces {
    public interface IProductPriceProvider { // поставщик цены продукта
        Task<decimal> GetPriceAsync(string url, CancellationToken cancellationToken);
        Task<string> GetTitleAsync(string url, CancellationToken cancellationToken);
    }
}
