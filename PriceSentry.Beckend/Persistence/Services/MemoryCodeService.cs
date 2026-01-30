using Org.BouncyCastle.Tsp;
using PriceSentry.Application.Interfaces;

namespace PriceSentry.Persistence.Services {
    public class MemoryCodeService : IStoregCodeService {

        private readonly Dictionary<string, (string Code, int Attempts, DateTime Expires)> _storage = new();
        public Task<bool> IsValidCodeAsync(string kay, string code, CancellationToken cancellationToken) {
            if(!_storage.ContainsKey(kay)) 
                return Task.FromResult(false);

            var (savedCode, attempts, expires) = _storage[kay];

            if(DateTime.UtcNow > expires)
                return Task.FromResult(false);

            if (attempts <= 0) 
                return Task.FromResult(false);
            
            if(savedCode != code) {
                _storage[kay] = (savedCode, attempts - 1, expires);
                return Task.FromResult(false);
            }

            _storage.Remove(kay);
            return Task.FromResult(true);
            
        }

        public Task RemoveCodeAsync(string kay, CancellationToken cancellationToken) {
            if(_storage.ContainsKey(kay))
                _storage.Remove(kay);
            return Task.CompletedTask;

        }

        public Task StoreCodeAsync(string kay, string code, CancellationToken cancellationToken) {
            _storage[kay] = (code, 5, DateTime.UtcNow.AddMonths(1));
            return Task.CompletedTask;
        }
    }
}
