using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PriceSentry.Application.Common.Behavior;
using PriceSentry.Application.Common.Exceptions;
using PriceSentry.Application.Common.Mappings;
using PriceSentry.Application.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Polling;
using TelegramBotHost.Configurations;
using TelegramBotHost.Handlers;
using TelegramBotHost.Services;

namespace PriceSentry.Persistence {
    public static class DependencyInjection {
        public static IServiceCollection AddTelegramBot(this IServiceCollection services, IConfiguration configuration) {
            var botToken = configuration["TelegramBot:Token"];
            if (string.IsNullOrEmpty(botToken))
                throw new InvalidOperationException("TelegramBot:Token is not configured");

            services.Configure<BotConfiguration>(configuration.GetSection("TelegramBot"));
            services.AddSingleton<ITelegramBotClient>(sp => new TelegramBotClient(botToken));
            services.AddSingleton<IUpdateHandler, UpdateHandler>();

            services.AddHostedService<TelegramBotService>();
            
            return services;
        }
    }
}

