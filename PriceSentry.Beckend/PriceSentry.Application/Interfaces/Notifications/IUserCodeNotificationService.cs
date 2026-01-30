using PriceSentry.Domain;

namespace PriceSentry.Application.Interfaces.Notifications {
    public interface IUserCodeNotificationService {
        public Task SendCodeNotificationAsync(string email, string code);


    }
}
