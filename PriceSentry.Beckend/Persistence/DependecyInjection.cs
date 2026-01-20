using MailKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PriceSentry.Application.Interfaces;
using PriceSentry.Persistence.Configuration;
using PriceSentry.Persistence.Services;
using PriceSentry.Persistence.Services.Notification;
using Telegram.Bot;

namespace PriceSentry.Persistence {
    public static class DependecyInjection {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration) {
            var connectionString = configuration["DbConnection"];
            var botToken = configuration["TelegramBot:Token"];

            if (string.IsNullOrEmpty(botToken))
                throw new InvalidOperationException("TelegramBot:Token is not configured");

            services.AddDbContext<PriceSentryDbContext>(opt => opt.UseSqlite(connectionString));
            services.AddScoped<IPriceSentryDbContext>(provider => provider.GetService<PriceSentryDbContext>());
            services.AddSingleton<ITelegramBotClient>(sp => new TelegramBotClient(botToken));
            services.AddScoped<ITrackingService, TracingService>();
            services.AddScoped<IPriceParserService, PriceParserService>();
            services.AddScoped<INotificationService, TelegramNotificationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<INotificationService, EmailNotificationService> ();
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddTransient<IMailService, MailService>();
            return services;
        }
    }
}
