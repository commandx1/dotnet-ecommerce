using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Reviews;

public sealed class Review : BaseAuditableEntity
{
    public Guid ProductId { get; set; }
    public Guid BuyerId { get; set; }
    public int Rating { get; set; }
    public required string Comment { get; set; }
    public string? ReplyText { get; set; }
}
