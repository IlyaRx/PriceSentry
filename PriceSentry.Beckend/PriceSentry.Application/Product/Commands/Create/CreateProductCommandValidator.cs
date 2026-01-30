
using FluentValidation;

namespace PriceSentry.Application.Product.Commands.Create {
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand> {
        public CreateProductCommandValidator() {
            RuleFor(createProductCommand => createProductCommand.UserId).NotEqual(Guid.Empty);
            RuleFor(createProductCommand => createProductCommand.DesiredPrice).GreaterThanOrEqualTo(0).LessThanOrEqualTo(1000000);
            RuleFor(createProductCommand => createProductCommand.ProductUrl).NotEmpty();

        }
    }
}

/*
📁 PriceSentry/
├── 📁 Project.Domain/
│   ├── ApplicationUser.cs
│   ├── ProductPriceHistory.cs
│   └── TrackingProduct.cs
├── 📁 Project.Application/
│   ├── 📁 Common/
│   │   ├── 📁 Behavior/
│   │   │   └── ValidationBehavior.cs
│   │   ├── 📁 Exceptions/
│   │   │   ├── NotFoundException.cs
│   │   │   └── NotificationException.cs
│   │   └── 📁 Mappings/
│   │        ├── AssemplyMappingProfile.cs
│   │        └── IMapWith.cs
│   ├── 📁 Interfaces/
│   │   ├── IEmailService.cs/
│   │   ├── INotificationService.cs/
│   │   ├── IPriceDropChecker.cs/
│   │   ├── IPriceParserService.cs/
│   │   ├── IPriceSentryDbContext.cs/
│   │   ├── IShopPriceParser.cs/
│   │   └── ITrackingService.cs/
│   ├── 📁 Price/
│   │   └── 📁 Queries/
│   │        └── 📁 GetPriceHistoryList/
│   │            ├── GetPriceHistoryQuery.cs
│   │            ├── GetPriceHistoryQueryHandler.cs
│   │            ├── GetPriceHistoryQueryValidator.cs
│   │            ├── PriceListVm.cs
│   │            └── PriceLookupDTO.cs
│   ├── 📁 Product/
│   │   ├── 📁 Commands/
│   │   │   ├── 📁 Create/
│   │   │   │   ├── CreateProductCommand.cs
│   │   │   │   ├── CreateProductCommandHandler.cs
│   │   │   │   └── CreateProductCommandValidator.cs
│   │   │   ├── 📁 Delete/
│   │   │   │   ├── DeleteProductCommand.cs
│   │   │   │   ├── DeleteProductCommandHandler.cs
│   │   │   │   └── DeleteProductCommandValidator.cs
│   │   │   └── 📁 GetPriceHistoryList/
│   │   │       ├── UpdateProductCommand.cs
│   │   │       ├── UpdateProductCommandHandler.cs
│   │   │       └── UpdateProductCommandValidator.cs
│   │   └── 📁 Queries/
│   │       ├── 📁 GetActualPrice/
│   │       │   ├── ActualPriceVm.cs
│   │       │   ├── GetActualPriceQuery.cs
│   │       │   ├── GetActualPriceQueryHandler.cs
│   │       │   └── GetActualPriceQueryValidator.cs
│   │       ├── 📁 GetListProducts/
│   │       │   ├── ProductListQuery.cs
│   │       │   ├── ProductListQueryHundler.cs
│   │       │   ├── ProductListQueryValidator.cs
│   │       │   ├── ProductListVm.cs
│   │       │   └── ProductLookupVm.cs
│   │       └── 📁 GetProduct/
│   │           ├── ProductDitailsQuery.cs
│   │           ├── ProductDitailsQueryHandler.cs
│   │           ├── ProductDitailsQueryValidator.cs
│   │           └── ProductDitailsVm.cs
│   ├── 📁 Validators/
│   │   └── PriceDropCheckerService.cs
│   └── DependencyInjecion.cs
├── 📁 Project.Infrastructure/
│   ├── 📁 Configuration/
│   │   └── MailSettings.cs
│   ├── 📁 EntityTypeConfiguration/
│   │   ├── PriceConfiguration.cs
│   │   ├── ProductConfiguration.cs
│   │   └── UserConfiguration.cs
│   ├── 📁 Services/
│   │   ├── 📁 Notification/
│   │   │   ├── EmailNotificationService.cs
│   │   │   ├── EmailService.cs
│   │   │   └── TelegramNotificationService.cs
│   │   ├── 📁 Shops/
│   │   ├── PriceParserService.cs
│   │   └── TracingService.cs
│   ├── DbInitializer.cs
│   ├── DependecyInjection.cs
│   └── PriceSentryDbContext.cs
├── 📁 Project.Web.API/
│   ├── 📁 Controllers/
│   │   ├── BaseController.cs/
│   │   └── PriceProductController.cs
│   ├── 📁 Models/
│   │   ├── BaseController.cs/
│   │   └── PriceProductController.cs
│   └── Program.cs
└── Project.sln


*/
