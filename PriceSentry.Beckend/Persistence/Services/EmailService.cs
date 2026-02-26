using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PriceSentry.Application.Interfaces;
using PriceSentry.Persistence.Configuration;


namespace PriceSentry.Persistence.Services {
    public class EmailService : IEmailService {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings) =>
            _mailSettings = mailSettings.Value;
        
        public async Task SendEmailAsync(string to, string subjec, string body) {

            using var emailMessege = new MimeMessage();

            emailMessege.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.From));
            emailMessege.To.Add(new MailboxAddress("", to));
            emailMessege.Subject = subjec;
            emailMessege.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using (var client = new SmtpClient()) {
                await client.ConnectAsync(_mailSettings.Host,
                                          _mailSettings.Port,
                                          //SecureSocketOptions.StartTls);
                                          _mailSettings.UseSSL ?
                                            SecureSocketOptions.SslOnConnect :
                                            SecureSocketOptions.None);
                await client.AuthenticateAsync(_mailSettings.UserName, 
                                             _mailSettings.Password);
                await client.SendAsync(emailMessege);
                await client.DisconnectAsync(true);
            }
        }
    }
}
