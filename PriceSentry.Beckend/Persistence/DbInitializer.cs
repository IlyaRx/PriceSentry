
namespace PriceSentry.Persistence {
    public class DbInitializer {
        public static void Initialize(PriceSentryDbContext context) => 
                context.Database.EnsureCreated();
    }
}
