using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Domain.Auth;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return Query().FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
    }
}
