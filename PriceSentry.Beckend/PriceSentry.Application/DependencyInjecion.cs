
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PriceSentry.Application.Common.Behavior;
using System.Buffers;
using System.Reflection;

namespace PriceSentry.Application {
    public static class DependencyInjecion {
        public static IServiceCollection AddApplication(this IServiceCollection services) {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssemblies(new[] {Assembly.GetExecutingAssembly()});
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
    }
}
