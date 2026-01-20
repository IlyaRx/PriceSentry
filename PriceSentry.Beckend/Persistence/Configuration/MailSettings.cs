
namespace PriceSentry.Persistence.Configuration {
    public class MailSettings {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
    }
}
/*
 {
  "MailSettings": {
    "Host": "smtp.gmail.com", // Адрес вашего SMTP-сервера
    "Port": 587,              // Обычно 587 (TLS) или 465 (SSL)
    "UseSSL": true,
    "UserName": "your-email@gmail.com",
    "Password": "", // Оставьте пустым, используйте User Secrets
    "DisplayName": "Ваше приложение",
    "From": "your-email@gmail.com"
  }
}
 
programm.cs
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IMailService, MailService>();
 */