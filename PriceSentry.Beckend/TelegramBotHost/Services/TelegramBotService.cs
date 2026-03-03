using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using TelegramBotHost.Interfases;

namespace TelegramBotHost.Services {
    public class TelegramBotService : BackgroundService, ITelegramBotSrevice {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<TelegramBotService> _logger;
        private readonly IUpdateHandler _updateHandler;

        public TelegramBotService(
            ITelegramBotClient botClient,
            ILogger<TelegramBotService> logger,
            IUpdateHandler updateHandler) {
            _botClient = botClient;
            _logger = logger;
            _updateHandler = updateHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            _logger.LogInformation("Бот запускается...");


            var me = await _botClient.GetMe(stoppingToken);
            _logger.LogInformation("Бот @{BotUsername} успешно подключен", me.Username);

            var receiverOptions = new ReceiverOptions {
                AllowedUpdates = Array.Empty<UpdateType>(), 
                DropPendingUpdates = true 
            };

            try {
                _botClient.StartReceiving(
                    updateHandler: _updateHandler,
                    receiverOptions: receiverOptions,
                    cancellationToken: stoppingToken
                );

                await Task.Delay(Timeout.Infinite, stoppingToken);
            } catch (OperationCanceledException) {
                _logger.LogWarning("Бот остановлен (отмена операции)");
            } catch (Exception ex) {
                _logger.LogError(ex, "Критическая ошибка бота");
            }
        }
    }
}