using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Api.Contracts;

public class CreateVendorProductRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    public IFormFile? Image { get; set; }
}

public class UpdateVendorProductRequest : CreateVendorProductRequest
{
}
