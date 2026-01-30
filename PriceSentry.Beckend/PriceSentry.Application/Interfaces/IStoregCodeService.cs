
namespace PriceSentry.Application.Interfaces {
    public interface IStoregCodeService {
        Task StoreCodeAsync(string kay, string code,CancellationToken cancellationToken);
        Task<bool> IsValidCodeAsync(string kay, string code,CancellationToken cancellationToken);

        Task RemoveCodeAsync(string email, CancellationToken cancellationToken);
    }
}
