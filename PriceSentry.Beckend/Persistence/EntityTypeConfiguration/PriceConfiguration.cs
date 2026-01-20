using PriceSentry.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PriceSentry.Persistence.EntityTypeConfiguration {
    public class PriceConfiguration : IEntityTypeConfiguration<ProductPriceHistory> {
        public void Configure(EntityTypeBuilder<ProductPriceHistory> builder) {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Id)
                   .IsUnique();

            builder.HasOne(x => x.TrackingProduct)
                   .WithMany(p => p.PriceHistory) 
                   .HasForeignKey(x => x.ProductId)
                   .OnDelete(DeleteBehavior.Cascade); 

            builder.Property(x => x.ProductId)
                   .IsRequired();

            builder.Property(x => x.Price)
                   .HasPrecision(7, 2)
                   .IsRequired();

            builder.Property(x => x.AddDate)
                   .IsRequired()
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasIndex(x => x.ProductId);
            builder.HasIndex(x => new { x.ProductId, x.AddDate });

            builder.ToTable(tb => tb.HasCheckConstraint("CK_ProductPriceHistory_Price", "\"Price\" > 0"));
        }
    }
}
