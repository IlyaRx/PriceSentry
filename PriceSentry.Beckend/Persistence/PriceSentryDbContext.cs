
using Microsoft.EntityFrameworkCore;
using PriceSentry.Application.Interfaces;
using PriceSentry.Domain;
using PriceSentry.Persistence.EntityTypeConfiguration;

namespace PriceSentry.Persistence {
    public class PriceSentryDbContext : DbContext, IPriceSentryDbContext {
        public DbSet<TrackingProduct> Products { get; set; }
        public DbSet<ProductPriceHistory> ProductPrices { get; set; }
        public DbSet<ApplicationUser> Users { get ; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfiguration(new PriceConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
