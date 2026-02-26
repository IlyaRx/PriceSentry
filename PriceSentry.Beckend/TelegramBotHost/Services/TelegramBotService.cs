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

            // Проверяем связь с Telegram
            var me = await _botClient.GetMe(stoppingToken);
            _logger.LogInformation("Бот @{BotUsername} успешно подключен", me.Username);

            // Запускаем опрос (Long Polling)
            var receiverOptions = new ReceiverOptions {
                AllowedUpdates = Array.Empty<UpdateType>(), // Все типы обновлений
                DropPendingUpdates = true // Пропустить старые сообщения при старте
            };

            try {
                _botClient.StartReceiving(
                    updateHandler: _updateHandler,
                    receiverOptions: receiverOptions,
                    cancellationToken: stoppingToken
                );

                // Держим сервис запущенным, пока не придет сигнал остановки
                await Task.Delay(Timeout.Infinite, stoppingToken);
            } catch (OperationCanceledException) {
                _logger.LogWarning("Бот остановлен (отмена операции)");
            } catch (Exception ex) {
                _logger.LogError(ex, "Критическая ошибка бота");
            }
        }
    }
}