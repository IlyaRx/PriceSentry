using PriceSentry.Application.Interfaces.Notifications;
using PriceSentry.Domain;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace PriceSentry.Persistence.Services.Notification {
    public class TelegramNotificationService : IPriceNotificationService {
        private readonly ITelegramBotClient _botClient;
        public TelegramNotificationService(ITelegramBotClient telemetryClient) => _botClient = telemetryClient;
        public async Task SendPriceDropNotificationAsync(TrackingProduct product, decimal newPrice) {
            if (product.User?.TelegramChatId == null)
                return;
            var message = $"🎉 *Цена на {product.Title} снизилась!*\n" +
                          $"💰 Новая цена: {newPrice} руб.\n" +
                          $"🎯 Ваша цель: {product.DesiredPrice} руб.\n" +
                          $"[Перейти к товару]({product.ProductUrl})";
            await _botClient.SendMessage(
                chatId: product.User.TelegramChatId.Value,
                text: message,
                parseMode: ParseMode.MarkdownV2
);
        }
    }
}
