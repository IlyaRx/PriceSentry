using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PriceSentry.Application.Interfaces;
using PriceSentry.Application.Interfaces.Notifications;
using PriceSentry.Domain;
using PriceSentry.Persistence.Configuration;
using PriceSentry.Persistence.Interfases;
using PriceSentry.Persistence.Services;
using PriceSentry.Persistence.Services.Notification;
using PriceSentry.Persistence.Services.Shops;
using System.Runtime.InteropServices;
using Telegram.Bot;

namespace PriceSentry.Persistence {
    public static class DependecyInjection {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration) {
            var connectionString = configuration["DbConnection"];
            var botToken = configuration["TelegramBot:Token"];

            services.AddDbContext<PriceSentryDbContext>(opt => opt.UseSqlite(connectionString));
            services.AddScoped<IPriceSentryDbContext>(provider => provider.GetService<PriceSentryDbContext>());

            services.AddScoped<ITrackingService, TracingService>();
            services.AddScoped<IProductPriceProvider, PriceParserService>();
            services.AddScoped<IPriceNotificationService, TelegramNotificationService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<IPriceNotificationService, EmailNotificationService>();
            services.AddScoped<IGeneratedCode, GeneratedCodeSetvice>();
            services.AddScoped<IUserCodeNotificationService, UserCodeEmailNotificationService>();
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddHostedService<TrackingBackgroundService>();
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddScoped<ITokenService, JwtTokenService>();

            services.AddSingleton<IStoregCodeService, MemoryCodeService>();
            services.AddSingleton<IShopPriceParser, CitilinkParserPrice>();

            if (string.IsNullOrEmpty(botToken))
                throw new InvalidOperationException("TelegramBot:Token is not configured");

            services.AddSingleton<ITelegramBotClient>(sp => new TelegramBotClient(botToken));
            return services;
        }
    }
}

