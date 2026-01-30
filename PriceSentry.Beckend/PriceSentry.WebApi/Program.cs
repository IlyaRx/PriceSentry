using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PriceSentry.Application;
using PriceSentry.Application.Common.Mappings;
using PriceSentry.Application.Interfaces;
using PriceSentry.Domain;
using PriceSentry.Persistence;
using PriceSentry.Persistence.Interfases;
using PriceSentry.Persistence.Services;
using System.Net;
using System.Reflection;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();


builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
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
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
    };
});

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

app.UseHttpsRedirection();
app.UseStaticFiles(); 
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();
