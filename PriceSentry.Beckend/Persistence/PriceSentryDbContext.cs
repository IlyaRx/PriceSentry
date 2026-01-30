
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PriceSentry.Application.Interfaces;
using PriceSentry.Domain;
using PriceSentry.Persistence.EntityTypeConfiguration;

namespace PriceSentry.Persistence {
    public class PriceSentryDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IPriceSentryDbContext{

        public PriceSentryDbContext(DbContextOptions<PriceSentryDbContext> options) : base(options) {}
        public PriceSentryDbContext() {}

        public DbSet<TrackingProduct> Products { get; set; }
        public DbSet<ProductPriceHistory> ProductPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new PriceConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
 	        modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
