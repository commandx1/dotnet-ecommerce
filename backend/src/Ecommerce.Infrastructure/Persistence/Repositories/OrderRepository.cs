using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Order>> GetBuyerOrdersAsync(Guid buyerId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .AsNoTracking()
            .Where(order => order.BuyerId == buyerId)
            .Include(order => order.Items)
            .OrderByDescending(order => order.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetOrdersContainingProductsAsync(
        IReadOnlyCollection<Guid> productIds,
        CancellationToken cancellationToken = default)
    {
        return await Query()
            .AsNoTracking()
            .Where(order => order.Items.Any(item => productIds.Contains(item.ProductId)))
            .Include(order => order.Items)
            .OrderByDescending(order => order.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
