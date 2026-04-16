using Ecommerce.Domain.Orders;

namespace Ecommerce.Application.Common.Dtos;

public sealed record OrderItemDto(Guid ProductId, int Quantity, decimal UnitPrice);

public sealed record BuyerOrderDto(
    Guid Id,
    OrderStatus Status,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    IReadOnlyList<OrderItemDto> Items);

public sealed record VendorOrderDto(
    Guid Id,
    Guid BuyerId,
    OrderStatus Status,
    DateTimeOffset CreatedAt,
    decimal VendorTotal,
    IReadOnlyList<OrderItemDto> Items);

public sealed record CheckoutOrderResultDto(Guid OrderId, decimal TotalAmount, OrderStatus Status);
