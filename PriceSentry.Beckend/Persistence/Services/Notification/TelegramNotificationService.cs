using Microsoft.AspNetCore.Identity;
using PriceSentry.Application.Interfaces.Notifications;
using PriceSentry.Domain;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace PriceSentry.Persistence.Services.Notification {
    public class TelegramNotificationService : IPriceNotificationService {
        private readonly UserManager<ApplicationUser>_userManager;
        private readonly ITelegramBotClient _botClient;
        public TelegramNotificationService(ITelegramBotClient telemetryClient, UserManager<ApplicationUser> userManager = null) {
            _botClient = telemetryClient;
            _userManager = userManager;
        }

        public async Task SendPriceDropNotificationAsync(TrackingProduct product, decimal newPrice) {
            var user = await _userManager.FindByIdAsync(product.UserId.ToString()) ?? null;
            if(user == null || user.TelegramChatId == null)
                return;
            var message = $"🎉 *Цена на {product.Title} снизилась!\n" +
                          $"💰 Новая цена: {newPrice} руб.\n" +
                          $"🎯 Ваша цель: {product.DesiredPrice} руб.\n" +
                          $"Перейти к товару >> ({product.ProductUrl})";
            await _botClient.SendMessage(
                chatId: user.TelegramChatId.Value,
                text: message
            );
        }
    }
}
