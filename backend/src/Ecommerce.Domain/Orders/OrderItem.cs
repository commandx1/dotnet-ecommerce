using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Orders;

public sealed class OrderItem : BaseAuditableEntity
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
