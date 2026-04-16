using Ecommerce.Domain.Orders;

namespace Ecommerce.Application.Abstractions.Persistence;

public interface IOrderRepository : IRepository<Order>
{
    Task<IReadOnlyList<Order>> GetBuyerOrdersAsync(Guid buyerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetOrdersContainingProductsAsync(
        IReadOnlyCollection<Guid> productIds,
        CancellationToken cancellationToken = default);
}
