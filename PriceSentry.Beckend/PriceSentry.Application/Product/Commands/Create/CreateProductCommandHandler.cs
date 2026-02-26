using AutoMapper.Internal.Mappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PriceSentry.Application.Common.Exceptions;
using PriceSentry.Application.Interfaces;
using PriceSentry.Domain;

namespace PriceSentry.Application.Product.Commands.Create {
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid> {
        private readonly IPriceSentryDbContext _dbContext;
        private readonly IProductPriceProvider productPriceProvider;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateProductCommandHandler(IPriceSentryDbContext dbContext, IProductPriceProvider priceParserService, UserManager<ApplicationUser> userManager) {
            (_dbContext, productPriceProvider) = (dbContext, priceParserService);
            _userManager = userManager;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken) {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductUrl == request.ProductUrl) ?? null;

            if(product != null)
                return product.Id;

            var priceTask = productPriceProvider.GetPriceAsync(request.ProductUrl, cancellationToken);
            var titleTask = productPriceProvider.GetTitleAsync(request.ProductUrl, cancellationToken);
            await Task.WhenAll(priceTask, titleTask);

            product = new TrackingProduct {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ProductUrl = request.ProductUrl,
                DesiredPrice = request.DesiredPrice,
                ActualPrice = await priceTask,
                Title = await titleTask,
                LastTracking = DateTime.Now,
                User = user!
            };
            var price = new ProductPriceHistory {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                AddDate = DateTime.UtcNow,
                Price = product.ActualPrice,
                TrackingProduct = product,
            };
            

            await _dbContext.Products.AddAsync(product, cancellationToken);
            await _dbContext.ProductPrices.AddAsync(price, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return product.Id;
        }
    }
}
