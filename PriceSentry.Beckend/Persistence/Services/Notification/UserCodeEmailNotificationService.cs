
using Microsoft.EntityFrameworkCore;
using PriceSentry.Application.Common.Exceptions;
using PriceSentry.Application.Interfaces;
using PriceSentry.Application.Interfaces.Notifications;

namespace PriceSentry.Persistence.Services.Notification {
    public class UserCodeEmailNotificationService : IUserCodeNotificationService {
        private readonly IEmailService _emailSender;

        public UserCodeEmailNotificationService(IEmailService emailSender) {
            _emailSender = emailSender;
        }

        public async Task SendCodeNotificationAsync(string email, string code) {

            if (string.IsNullOrEmpty(email)) {
                throw new NotificationException($"Не удалось отправить код, неправильно написана почта.");
            }
            var subject = $"Код для входа на сайт!";
            var body = $"<h3>Код для подтверждения {code}, никому его не говорите!</h3>";
            await _emailSender.SendEmailAsync(email, subject, body);
        }
    }
}
