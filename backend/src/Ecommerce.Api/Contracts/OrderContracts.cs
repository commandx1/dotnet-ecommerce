using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Api.Contracts;

public sealed class CheckoutItemRequest
{
    [Required]
    public Guid ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

public sealed class CheckoutRequest
{
    [Required]
    [MinLength(1)]
    public List<CheckoutItemRequest> Items { get; set; } = [];
}
