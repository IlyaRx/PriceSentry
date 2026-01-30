//using Microsoft.Playwright;
//using PriceSentry.Application.Interfaces;
//using PriceSentry.Persistence.Interfases;
//using System.Net;
//using System.Text;
//using System.Xml;
//using static System.Net.Mime.MediaTypeNames;

//namespace PriceSentry.Persistence.Services {
//    public class PriceParserService : IProductPriceProvider, IAsyncDisposable {
//        private readonly IEnumerable<IShopPriceParser> _shopPriceParsers;
//        private IPlaywright _playwright;
//        private IBrowser _browser;
//        private bool _isInitialized = false;
//        private readonly object _lock = new object();

//        // Конструктор БЕЗ Playwright параметров
//        public PriceParserService(IEnumerable<IShopPriceParser> shopPriceParsers) {
//            _shopPriceParsers = shopPriceParsers;
//        }

//        private async Task InitializeAsync() {
//            if (_isInitialized) return;

//            lock (_lock) {
//                if (_isInitialized) return;
//            }

//            try {
//                // Устанавливаем Playwright если не установлен
//                var exitCode = Microsoft.Playwright.Program.Main(new[] { "install" });
//                if (exitCode != 0) {
//                    throw new InvalidOperationException($"Playwright installation failed with exit code {exitCode}");
//                }

//                _playwright = await Playwright.CreateAsync();
//                _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions {
//                    Headless = true,
//                    Args = new[] {
//                        "--disable-blink-features=AutomationControlled",
//                        "--disable-dev-shm-usage",
//                        "--no-sandbox"
//                    }
//                });

//                _isInitialized = true;
//                Console.WriteLine("Playwright initialized successfully");
//            } catch (Exception ex) {
//                throw new InvalidOperationException("Failed to initialize Playwright. Run 'pwsh bin/Debug/net9.0/playwright.ps1 install' manually", ex);
//            }
//        }

//        public async Task<decimal> GetPriceAsync(string url, CancellationToken cancellationToken) {
//            await InitializeAsync();

//            try {
//                var html = await GetHtmlWithPlaywrightAsync(url, cancellationToken);

//                var parser = _shopPriceParsers.FirstOrDefault(p => p.CanParse(url));
//                if (parser == null)
//                    throw new NotSupportedException($"No parser found for URL: {url}");

//                return await parser.ParsePriceAsync(html, cancellationToken);
//            } catch (Exception ex) {
//                Console.WriteLine($"Error getting price from {url}: {ex.Message}");
//                throw;
//            }
//        }

//        public async Task<string> GetTitleAsync(string url, CancellationToken cancellationToken) {
//            await InitializeAsync();

//            try {
//                var html = await GetHtmlWithPlaywrightAsync(url, cancellationToken);

//                var parser = _shopPriceParsers.FirstOrDefault(p => p.CanParse(url));
//                if (parser == null)
//                    throw new NotSupportedException($"No parser found for URL: {url}");

//                return await parser.ParseTitleAsync(html, cancellationToken);
//            } catch (Exception ex) {
//                Console.WriteLine($"Error getting title from {url}: {ex.Message}");
//                throw;
//            }
//        }

//        private async Task<string> GetHtmlWithPlaywrightAsync(string url, CancellationToken cancellationToken) {
//            IBrowserContext context = null;
//            IPage page = null;

//            try {
//                context = await _browser.NewContextAsync(new BrowserNewContextOptions {
//                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
//                    ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
//                    Locale = "ru-RU",
//                    TimezoneId = "Europe/Moscow",
//                    JavaScriptEnabled = true
//                });

//                // Добавляем куки Ozon
//                await context.AddCookiesAsync(new[]
//                {
//                    new Cookie
//                    {
//                        Name = "ozon_accept_cookie",
//                        Value = "true",
//                        Domain = ".ozon.ru",
//                        Path = "/"
//                    }
//                });

//                page = await context.NewPageAsync();

//                // Устанавливаем заголовки
//                await page.SetExtraHTTPHeadersAsync(new Dictionary<string, string> {
//                    ["Accept-Language"] = "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7",
//                    ["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8"
//                });

//                // Переходим на страницу
//                var response = await page.GotoAsync(url, new PageGotoOptions {
//                    WaitUntil = WaitUntilState.DOMContentLoaded,
//                    Timeout = 30000
//                });

//                if (response == null)
//                    throw new HttpRequestException($"Failed to load page: {url}");

//                // Ждем загрузки
//                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
//                await Task.Delay(2000); // Даем время на выполнение JS

//                // Проверяем антибот
//                var content = await page.ContentAsync();
//                if (IsAntibotPage(content)) {
//                    await Task.Delay(3000);
//                    content = await page.ContentAsync();
//                }

//                return content;
//            } finally {
//                if (page != null)
//                    await page.CloseAsync();
//                if (context != null)
//                    await context.CloseAsync();
//            }
//        }

//        private bool IsAntibotPage(string html) {
//            if (string.IsNullOrEmpty(html) || html.Length < 500)
//                return true;

//            var lowerHtml = html.ToLowerInvariant();

//            return lowerHtml.Contains("доступ ограничен") ||
//                   lowerHtml.Contains("antibot") ||
//                   lowerHtml.Contains("challenge") ||
//                   lowerHtml.Contains("докажите что вы человек") ||
//                   html.Contains("Antibot Challenge");
//        }

//        public async ValueTask DisposeAsync() {
//            if (_browser != null)
//                await _browser.DisposeAsync();

//            _playwright?.Dispose();
//        }
//    }
//}