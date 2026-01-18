using Microsoft.EntityFrameworkCore;
using PriceSentry.Domain;


namespace PriceSentry.Application.Interfaces {
    public interface IPriceSentryDbContext {
        DbSet<TrackingProduct> Products { get; set; }

        DbSet<ProductPriceHistory> ProductPrices {  get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
