using Ecommerce.Domain.Catalog;

namespace Ecommerce.Application.Abstractions.Persistence;

public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> GetPublicProductsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetByVendorAsync(Guid vendorId, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdForVendorAsync(Guid productId, Guid vendorId, CancellationToken cancellationToken = default);
}
