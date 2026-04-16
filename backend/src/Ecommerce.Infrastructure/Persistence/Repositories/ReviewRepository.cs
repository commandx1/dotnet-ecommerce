using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Domain.Reviews;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public sealed class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Review>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .AsNoTracking()
            .Where(review => review.ProductId == productId)
            .OrderByDescending(review => review.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Review>> GetByVendorAsync(Guid vendorId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .AsNoTracking()
            .Where(review => DbContext.Products.Any(product => product.Id == review.ProductId && product.VendorId == vendorId))
            .OrderByDescending(review => review.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<Review?> GetByIdForVendorAsync(Guid reviewId, Guid vendorId, CancellationToken cancellationToken = default)
    {
        return Query().FirstOrDefaultAsync(
            review => review.Id == reviewId
                && DbContext.Products.Any(product => product.Id == review.ProductId && product.VendorId == vendorId),
            cancellationToken);
    }
}
