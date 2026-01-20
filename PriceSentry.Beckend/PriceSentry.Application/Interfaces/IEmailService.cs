
namespace PriceSentry.Application.Interfaces {
    public interface IEmailService {
        Task SendEmailAsync(string to, string subjec, string body);
    }
}
