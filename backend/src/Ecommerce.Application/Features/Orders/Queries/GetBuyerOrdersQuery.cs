using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Common.Dtos;
using MediatR;

namespace Ecommerce.Application.Features.Orders.Queries;

public sealed record GetBuyerOrdersQuery(Guid BuyerId) : IRequest<IReadOnlyList<BuyerOrderDto>>;

public sealed class GetBuyerOrdersQueryHandler : IRequestHandler<GetBuyerOrdersQuery, IReadOnlyList<BuyerOrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetBuyerOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<BuyerOrderDto>> Handle(GetBuyerOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetBuyerOrdersAsync(request.BuyerId, cancellationToken);

        return orders
            .Select(order => new BuyerOrderDto(
                order.Id,
                order.Status,
                order.TotalAmount,
                order.CreatedAt,
                order.Items
                    .Select(item => new OrderItemDto(item.ProductId, item.Quantity, item.UnitPrice))
                    .ToList()))
            .ToList();
    }
}
