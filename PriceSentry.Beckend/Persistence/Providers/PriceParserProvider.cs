using Microsoft.Extensions.Logging;
using PriceSentry.Application.Interfaces;
using PriceSentry.Persistence.Interfases;
using System.IO.Compression;
using System.Text;

namespace PriceSentry.Persistence.Providers {
    public class PriceParserProvider : IProductPriceProvider {
        private readonly IEnumerable<IShopPriceParser> _shopPriceParser;
        private readonly ILogger<PriceParserProvider> _logger;

        public PriceParserProvider(IEnumerable<IShopPriceParser> shopPriceParser, 
                                  ILogger<PriceParserProvider> logger) {
            _shopPriceParser = shopPriceParser;
            _logger = logger;
        }


        public async Task<decimal> GetPriceAsync(string url, CancellationToken cancellationToken) {
            _logger.LogInformation("Поиск парсера для URL: {Url}", url);
            var parser = _shopPriceParser.FirstOrDefault(p => p.CanParse(url));
            if (parser == null) {
                _logger.LogError("Не найден парсер для URL: {Url}", url);
                throw new NotSupportedException($"Не найден парсер для URL: {url}");
            }

            _logger.LogInformation("Найден парсер {ParserType} для {Url}", parser.GetType().Name, url);

            try {
                var price = await parser.ParsePriceAsync(url, cancellationToken);
                _logger.LogInformation("Успешно получена цена {Price} руб. для {Url}", price, url);
                return price;

            } catch (Exception ex) {
                _logger.LogError(ex, "Ошибка при получении цены для {Url}", url);
                throw;
            }

        }

        public async Task<string> GetTitleAsync(string url, CancellationToken cancellationToken) {
            _logger.LogInformation("Поиск парсера для получения названия: {Url}", url);

            var parser = _shopPriceParser.FirstOrDefault(p => p.CanParse(url));
            if (parser == null) {
                _logger.LogError("Не найден парсер для URL: {Url}", url);
                throw new NotSupportedException($"Не найден парсер для URL: {url}");
            }

            _logger.LogInformation("Найден парсер {ParserType} для получения названия {Url}", parser.GetType().Name, url);

            try {
                var title = await parser.ParseTitleAsync(url, cancellationToken);
                _logger.LogInformation("Успешно получено название '{Title}' для {Url}", title, url);
                return title;
            } catch (Exception ex) {
                _logger.LogError(ex, "Ошибка при получении названия для {Url}", url);
                throw;
            }
        }
    }
}
