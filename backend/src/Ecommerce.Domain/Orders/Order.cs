using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Orders;

public sealed class Order : BaseAuditableEntity
{
    public Guid BuyerId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
