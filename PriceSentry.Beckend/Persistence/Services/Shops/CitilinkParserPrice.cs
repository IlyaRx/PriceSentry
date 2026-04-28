using Microsoft.Extensions.Logging;
using PriceSentry.Application.Common.Exceptions;
using PriceSentry.Persistence.Interfases;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PriceSentry.Persistence.Services.Shops {
    public class CitilinkParserPrice : BaseShopParser {
        public CitilinkParserPrice(HttpClient httpClient, ILogger<IShopPriceParser> logger) : base(httpClient, logger) { }

        public override bool CanParse(string url) {
            if(url is null) return false;
            bool can = url.Contains("citilink.ru");
            _logger.LogDebug("Проверка URL {Url} для Citilink: {Result}", url, can ? "да" : "нет");
            return can;
        }

        public override async Task<decimal> ParsePriceAsync(string url, CancellationToken cancellationToken) {
            _logger.LogInformation("Начинаю парсинг цены с Citilink: {Url}", url);
            var html = await FetchHtmlAsync(url, cancellationToken);

            if (string.IsNullOrEmpty(html)) {
                _logger.LogError("Не удалось получить HTML-код страницы {Url}", url);
                throw new NotFoundException("цена", url);
            }


            var match = Regex.Match(html, @"data-meta-price=""([\d\s.,]+)""");
            if (match.Success) {
                var priceStr = match.Groups[1].Value.Replace(" ", "");
                var price = decimal.Parse(priceStr);
                _logger.LogInformation("Цена товара {Url} успешно получена: {Price} руб.", url, price);
                return price;
            }
            _logger.LogWarning("Не удалось найти цену на странице {Url}. Возможно, изменилась структура HTML.", url);
            throw new NotFoundException("price", html);

        }

        public override async Task<string> ParseTitleAsync(string url, CancellationToken cancellationToken) {
            _logger.LogInformation("Начинаю парсинг названия товара с Citilink: {Url}", url);
            var html = await FetchHtmlAsync(url, cancellationToken);

            if (string.IsNullOrEmpty(html)) {
                _logger.LogError("Не удалось получить HTML-код страницы {Url}", url);
                throw new NotFoundException("название", url);
            }


            var match = Regex.Match(html, @"<h1[^>]*class=""[^""]*StyledProductTitle[^""]*""[^>]*>([^<]+)</h1>");
            if (match.Success) {
                var title = match.Groups[1].Value.Replace("\\", "").Trim();
                _logger.LogInformation("Название товара {Url} успешно получено: {Title}", url, title);
                return title;
            }
            _logger.LogWarning("Не удалось найти название товара на странице {Url}. Возможно, изменилась структура HTML.", url);
            throw new NotFoundException("Title", html);
        }

    }
}
