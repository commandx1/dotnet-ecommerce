using Ecommerce.Application.Common.Dtos;
using Ecommerce.Domain.Catalog;

namespace Ecommerce.Application.Features.Products;

public static class ProductMappings
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.ImageUrl,
            product.VendorId,
            product.CreatedAt,
            product.UpdatedAt);
    }
}
