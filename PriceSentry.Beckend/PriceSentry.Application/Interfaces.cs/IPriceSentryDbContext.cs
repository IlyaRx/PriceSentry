using Microsoft.EntityFrameworkCore;
using PriceSentry.Domain;


namespace PriceSentry.Application.Interfaces.cs {
    public interface IPriceSentryDbContext {
        DbSet<TrackingProduct> Product { get; set; }

        DbSet<ProductPriceHistory> ProductPrice {  get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
