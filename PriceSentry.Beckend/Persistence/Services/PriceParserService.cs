
using PriceSentry.Application.Interfaces;
using PriceSentry.Persistence.Interfases;

namespace PriceSentry.Persistence.Services {
    public class PriceParserService : IPriceParserService {
        private readonly HttpClient _httpClient;
        private readonly IEnumerable<IShopPriceParser> _shopPriceParser;

        public PriceParserService(HttpClient httpClient, IEnumerable<IShopPriceParser> shopPriceParser) {
            _httpClient = httpClient;
            _shopPriceParser = shopPriceParser;
        }
        

        public async Task<decimal> GetPriceAsync(string url, CancellationToken cancellationToken) {
            var html = await _httpClient.GetStringAsync(url, cancellationToken);

            var parser = _shopPriceParser.FirstOrDefault(p => p.CanParse(url));
            if (parser == null) 
                throw new NotSupportedException($"No parser found for URL: {url}");
            
            return await parser.ParsePriceAsync(html, cancellationToken);
        }

        public async Task<string> GetTitleAsync(string url, CancellationToken cancellationToken) {
            var html = await _httpClient.GetStringAsync(url, cancellationToken);

            var parser = _shopPriceParser.FirstOrDefault(p => p.CanParse(url));
            if (parser == null) 
                throw new NotSupportedException($"No parser found for URL: {url}");
            

            return await parser.ParseTtileAsync(html, cancellationToken);
        }
    }
}
