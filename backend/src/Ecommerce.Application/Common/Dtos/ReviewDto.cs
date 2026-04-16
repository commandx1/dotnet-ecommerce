namespace Ecommerce.Application.Common.Dtos;

public sealed record ReviewDto(
    Guid Id,
    Guid ProductId,
    Guid BuyerId,
    int Rating,
    string Comment,
    string? ReplyText,
    DateTimeOffset CreatedAt);
