using PriceSentry.Application.Interfaces;
using PriceSentry.Persistence.Interfases;
using System.IO.Compression;
using System.Text;

namespace PriceSentry.Persistence.Services {
    public class PriceParserService : IProductPriceProvider {
        private readonly IEnumerable<IShopPriceParser> _shopPriceParser;

        public PriceParserService(IEnumerable<IShopPriceParser> shopPriceParser) {
            _shopPriceParser = shopPriceParser;
        }


        public async Task<decimal> GetPriceAsync(string url, CancellationToken cancellationToken) {
            var parser = _shopPriceParser.FirstOrDefault(p => p.CanParse(url));
            if (parser == null)
                throw new NotSupportedException($"No parser found for URL: {url}");
            var a = await parser.ParsePriceAsync(url, cancellationToken);
            return a;
        }

        public async Task<string> GetTitleAsync(string url, CancellationToken cancellationToken) {
            var parser = _shopPriceParser.FirstOrDefault(p => p.CanParse(url));
            if (parser == null)
                throw new NotSupportedException($"No parser found for URL: {url}");
            var a = await parser.ParseTitleAsync(url, cancellationToken);
            return a;
        }
    }
}
