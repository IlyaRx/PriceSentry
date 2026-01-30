
using PriceSentry.Persistence.Interfases;
using System.IO.Compression;
using System.Net.Http;
using System.Text;

namespace PriceSentry.Persistence.Services.Shops {
    public abstract class BaseShopParser : IShopPriceParser {
        protected readonly HttpClient _httpClient;

        protected BaseShopParser(HttpClient httpClient) {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        }
        public abstract bool CanParse(string url);
        public abstract Task<decimal> ParsePriceAsync(string url, CancellationToken cancellationToken);
        public abstract Task<string> ParseTitleAsync(string url, CancellationToken cancellationToken);

        public virtual async Task<string> FetchHtmlAsync(string url, CancellationToken cancellationToken) {

            var response = await _httpClient.GetAsync(url, cancellationToken);
            var bytes = await response.Content.ReadAsByteArrayAsync();

            string html;
            if (response.Content.Headers.ContentEncoding.Contains("gzip")) {
                using var stream = new MemoryStream(bytes);
                using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
                using var reader = new StreamReader(gzipStream);
                html = await reader.ReadToEndAsync();
            }
            else if (response.Content.Headers.ContentEncoding.Contains("deflate")) {
                using var stream = new MemoryStream(bytes);
                using var deflateStream = new DeflateStream(stream, CompressionMode.Decompress);
                using var reader = new StreamReader(deflateStream);
                html = await reader.ReadToEndAsync();
            }
            else {
                html = Encoding.UTF8.GetString(bytes);
            }
            return html;
        }

    }
}
