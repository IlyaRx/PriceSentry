using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceSentry.Domain;

namespace PriceSentry.Persistence.EntityTypeConfiguration {
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser> {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder) {
            builder.HasKey(u => u.Id);
            builder.HasIndex(u => u.Id).IsUnique();

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(256);

            builder.HasIndex(u => u.Email)
                   .IsUnique();

            builder.HasIndex(u => u.TelegramChatId)
                   .IsUnique();

            builder.HasMany(u => u.TrackingProducts)
                   .WithOne(tp => tp.User)
                   .HasForeignKey(tp => tp.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
