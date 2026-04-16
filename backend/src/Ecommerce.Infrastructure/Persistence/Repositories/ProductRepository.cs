using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Product>> GetPublicProductsAsync(CancellationToken cancellationToken = default)
    {
        return await Query()
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetByVendorAsync(Guid vendorId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .AsNoTracking()
            .Where(x => x.VendorId == vendorId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetByIdsAsync(IReadOnlyCollection<Guid> productIds, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(product => productIds.Contains(product.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<Product?> GetByIdForVendorAsync(Guid productId, Guid vendorId, CancellationToken cancellationToken = default)
    {
        return Query()
            .FirstOrDefaultAsync(x => x.Id == productId && x.VendorId == vendorId, cancellationToken);
    }
}
