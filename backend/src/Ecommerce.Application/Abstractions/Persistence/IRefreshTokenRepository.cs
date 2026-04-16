using Ecommerce.Domain.Auth;

namespace Ecommerce.Application.Abstractions.Persistence;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
}
