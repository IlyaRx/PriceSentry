using PriceSentry.Application.Common.Exceptions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace PriceSentry.Persistence.Services.Shops {
    public class CitilinkParserPrice : BaseShopParser {
        public CitilinkParserPrice(HttpClient httpClient) : base(httpClient) { }

        public override bool CanParse(string url) => url.Contains("citilink.ru");

        public override async Task<decimal> ParsePriceAsync(string url, CancellationToken cancellationToken) {
            var html = await FetchHtmlAsync(url, cancellationToken);

            var match = Regex.Match(html, @"data-meta-price=""([\d\s]+)""");
            if (match.Success) {
                var priceStr = match.Groups[1].Value.Replace(" ", "");
               return decimal.Parse(priceStr);
            }
            throw new NotFoundException("price", html);
        }

        public override async Task<string> ParseTitleAsync(string url, CancellationToken cancellationToken) {
            var html = await FetchHtmlAsync(url, cancellationToken);

            var match = Regex.Match(html, @"<h1[^>]*class=""[^""]*StyledProductTitle[^""]*""[^>]*>([^<]+)</h1>");
            if (match.Success) {
                var priceStr = match.Groups[1].Value.Replace("\\", ""); ;
                return priceStr;
            }
            throw new NotFoundException("Title", html);
        }

    }
}
