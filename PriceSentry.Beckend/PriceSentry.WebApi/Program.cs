using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PriceSentry.Application;
using PriceSentry.Application.Common.Mappings;
using PriceSentry.Application.Interfaces;
using PriceSentry.Domain;
using PriceSentry.Persistence;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text;


// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()                          
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)   
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
    .MinimumLevel.Override("Microsoft.Hosting", LogEventLevel.Warning)   
    .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Error)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.StaticFiles", LogEventLevel.Error)
    .MinimumLevel.Override("TelegramBotHost", LogEventLevel.Information) 
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                     theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
    .CreateLogger();

try {
    Log.Information("Запуск приложения...");

    DotNetEnv.Env.Load();

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();
    builder.Services.AddHttpClient();
    builder.Services.AddApplication();
    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddTelegramBot(builder.Configuration);
    builder.Services.AddControllers();
    builder.Services.AddHttpContextAccessor();


    builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options => {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 1;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;

        options.SignIn.RequireConfirmedAccount = true;
        options.SignIn.RequireConfirmedEmail = true;
    })
        .AddEntityFrameworkStores<PriceSentryDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET")!))
        };
    });

    builder.Services.AddSwaggerGen();

    builder.Services.AddAutoMapper(cfg => {
        cfg.AddProfile(new AssemplyMappingProfile(Assembly.GetExecutingAssembly()));
        cfg.AddProfile(new AssemplyMappingProfile(typeof(IPriceSentryDbContext).Assembly));
    });

    builder.Services.AddCors(opt => {
        opt.AddPolicy("AllowAll", pol => {
            pol.AllowAnyHeader();
            pol.AllowAnyMethod();
            pol.AllowAnyOrigin();
        });
    });

    var app = builder.Build();

    using (var scop = app.Services.CreateAsyncScope()) {
        var serviceProvider = scop.ServiceProvider;
        try {
            var context = serviceProvider.GetRequiredService<PriceSentryDbContext>();
            DbInitializer.Initialize(context);
            Console.WriteLine("SQLite база создана: pricesentry.db");
        } catch (Exception ex) {
            Console.WriteLine($"Ошибка инициализации БД: {ex.Message}");
        }
    }

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("Приложение запущено");
    app.Run();
} catch (Exception ex) {
    Log.Fatal(ex, "Приложение остановлено критической ошибкой");
} finally {
    Log.CloseAndFlush();
}