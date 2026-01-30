using PriceSentry.Domain;

namespace PriceSentry.Application.Interfaces {
    public interface ITokenService {
        Task<string> GenerateTokenAsync(ApplicationUser user, CancellationToken cancellationToken);
    }
}