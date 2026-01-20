using Microsoft.EntityFrameworkCore;
using PriceSentry.Application.Common.Exceptions;
using PriceSentry.Application.Interfaces;
using PriceSentry.Domain;

namespace PriceSentry.Persistence.Services.Notification {
    public class EmailNotificationService : INotificationService { 
        private readonly IPriceSentryDbContext _dbContext;
        private readonly IEmailService _emailSender;

        public EmailNotificationService(IPriceSentryDbContext dbContext, IEmailService emailSender) =>
            (_dbContext, _emailSender) = (dbContext, emailSender);
        

        public async Task SendPriceDropNotificationAsync(TrackingProduct product, decimal newPrice) {
            var productWithUser = await _dbContext.Products.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == product.Id);

            if(productWithUser?.User == null || string.IsNullOrEmpty(productWithUser.User.Email)) {
                throw new NotificationException( $"Не удалось отправить уведомление о снижении цены для продукта {product.Id}. " +
                                                  "Пользователь не найден или email отсутствует.");
            }

            var userEmail = productWithUser.User.Email;
            var subject = $"Цена на {product.Title} снизилась до {product.DesiredPrice}!";
            var body = $"<h3>Поздравляем! Цена на товар '{product.Title}' теперь составляет " +
                       $"{newPrice} рублей — именно столько вы и хотели!</h3><p>Это ваша ожидаемая цена. " +
                       $"Не упустите момент для покупки!</p><p><a href='{product.ProductUrl}'>Перейти к товару →</a></p>";
            await _emailSender.SendEmailAsync(userEmail, subject, body);

        }
    }
}
