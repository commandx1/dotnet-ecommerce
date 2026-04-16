namespace Ecommerce.Application.Common.Dtos;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string? ImageUrl,
    Guid VendorId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
