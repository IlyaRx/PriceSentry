using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PriceSentry.Application.Interfaces;
using PriceSentry.Domain;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TelegramBotHost.Handlers {
    public class UpdateHandler : IUpdateHandler {
        private readonly IServiceScopeFactory _scopeFactory;

        public UpdateHandler(IServiceScopeFactory scopeFactory) {
            _scopeFactory = scopeFactory;
        }



        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {



            if (update.Message is not { } message) return;
            if (message.Text is not { } text) return;

            using var scope = _scopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<IPriceSentryDbContext>();

            if (text.StartsWith("/start")) {

                var parts = text.Split(' ', 2);
                var parameter = parts.Length > 1 ? parts[1].Trim() : null;

                if (!string.IsNullOrEmpty(parameter)) {

                    if (Guid.TryParse(parameter, out var userId)) {
                        var user = await userManager.FindByIdAsync(userId.ToString());
                        if (user != null) {
                            user.TelegramChatId = message.Chat.Id;
                            await userManager.UpdateAsync(user);
                            await dbContext.SaveChangesAsync(cancellationToken);

                            await botClient.SendMessage(
                                chatId: message.Chat.Id,
                                text: "✅ Аккаунт успешно привязан!\nТеперь вы будете получать уведомления о ценах.",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        await botClient.SendMessage(
                            chatId: message.Chat.Id,
                            text: "❌ Пользователь не найден. Проверьте ссылку.",
                            cancellationToken: cancellationToken);
                        return;
                    }

                    await botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "❌ Неверный формат ссылки.",
                        cancellationToken: cancellationToken);
                    return;
                }

                // Если параметра нет — просто приветствие
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "👋 Привет! Я PriceSentry Bot.\n\nЧтобы привязать аккаунт, используйте ссылку из личного кабинета.",
                    cancellationToken: cancellationToken);
                return;
            }

            //if (text.StartsWith("/start")) {
            //    await botClient.SendMessage(
            //        chatId: message.Chat.Id,
            //        text: "Привет! Я PriceSentry Bot.\nОтправь мне код привязки, который ты получил на сайте.",
            //        cancellationToken: cancellationToken);
            //}
            //else {
            //    // Здесь будет логика проверки кода привязки
            //    // 1. Получить код из текста
            //    // 2. Проверить в Redis
            //    // 3. Если ок -> записать ChatId в БД пользователю
            //    await botClient.SendMessage(
            //        chatId: message.Chat.Id,
            //        text: $"Получил код: {text}. (Здесь будет проверка)",
            //        cancellationToken: cancellationToken);
            //}
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }
    }
}