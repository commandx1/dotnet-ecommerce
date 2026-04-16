using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Catalog;

public sealed class Product : BaseAuditableEntity
{
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
    public Guid VendorId { get; set; }
}
