
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceSentry.Domain;

namespace PriceSentry.Persistence.EntityTypeConfiguration {
    public class ProductConfiguration : IEntityTypeConfiguration<TrackingProduct> {
        public void Configure(EntityTypeBuilder<TrackingProduct> builder) {
            builder.HasKey(tp => tp.Id);
            builder.HasIndex(tp => tp.Id).IsUnique();

            builder.HasMany(tp => tp.PriceHistory)
                   .WithOne(pph => pph.TrackingProduct);

            builder.Property(tp => tp.UserId).IsRequired();
            builder.Property(tp => tp.DesiredPrice).HasPrecision(18, 2).IsRequired();
            builder.Property(tp => tp.ProductUrl).IsRequired().HasMaxLength(2048);
            builder.Property(tp => tp.ActualPrice).HasPrecision(18, 2).HasDefaultValue(0m);
            builder.Property(tp => tp.Title).HasMaxLength(250);

            builder.HasIndex(tp => tp.LastTracking);
            builder.HasIndex(tp => new { tp.UserId, tp.DesiredPrice });
        }
    }
}
