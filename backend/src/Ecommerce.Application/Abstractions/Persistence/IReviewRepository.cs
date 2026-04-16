using Ecommerce.Domain.Reviews;

namespace Ecommerce.Application.Abstractions.Persistence;

public interface IReviewRepository : IRepository<Review>
{
    Task<IReadOnlyList<Review>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Review>> GetByVendorAsync(Guid vendorId, CancellationToken cancellationToken = default);
    Task<Review?> GetByIdForVendorAsync(Guid reviewId, Guid vendorId, CancellationToken cancellationToken = default);
}
