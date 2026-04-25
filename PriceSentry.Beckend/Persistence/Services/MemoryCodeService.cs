using PriceSentry.Application.Interfaces;

namespace PriceSentry.Persistence.Services {
    public class MemoryCodeService : IStoregCodeService {

        private readonly ITimeProvider _timeProvider;

        public MemoryCodeService(ITimeProvider timeProvider) =>
            _timeProvider = timeProvider;
        

        private readonly Dictionary<string, (string Code, int Attempts, DateTime Expires)> _storage = new();
        public Task<bool> IsValidCodeAsync(string kay, string? code, CancellationToken cancellationToken) {
            if(!_storage.ContainsKey(kay)) 
                return Task.FromResult(false);

            var (savedCode, attempts, expires) = _storage[kay];

            if(code is null)   
                return Task.FromResult(false);

            if((_timeProvider.UtcNow > expires) || (attempts <= 0)) {
                _storage.Remove(kay);
                return Task.FromResult(false);
            }
            
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
            _storage[kay] = (code, 5, DateTime.UtcNow.AddDays(1));
            return Task.CompletedTask;
        }
    }
}
