using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Common.Dtos;
using MediatR;

namespace Ecommerce.Application.Features.Orders.Queries;

public sealed record GetVendorOrdersQuery(Guid VendorId) : IRequest<IReadOnlyList<VendorOrderDto>>;

public sealed class GetVendorOrdersQueryHandler : IRequestHandler<GetVendorOrdersQuery, IReadOnlyList<VendorOrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public GetVendorOrdersQueryHandler(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<VendorOrderDto>> Handle(GetVendorOrdersQuery request, CancellationToken cancellationToken)
    {
        var vendorProducts = await _productRepository.GetByVendorAsync(request.VendorId, cancellationToken);
        var vendorProductIds = vendorProducts.Select(x => x.Id).ToHashSet();

        if (vendorProductIds.Count == 0)
        {
            return [];
        }

        var orders = await _orderRepository.GetOrdersContainingProductsAsync(vendorProductIds.ToArray(), cancellationToken);

        return orders
            .Select(order =>
            {
                var vendorItems = order.Items
                    .Where(item => vendorProductIds.Contains(item.ProductId))
                    .Select(item => new OrderItemDto(item.ProductId, item.Quantity, item.UnitPrice))
                    .ToList();

                var vendorTotal = vendorItems.Sum(item => item.UnitPrice * item.Quantity);

                return new VendorOrderDto(
                    order.Id,
                    order.BuyerId,
                    order.Status,
                    order.CreatedAt,
                    vendorTotal,
                    vendorItems);
            })
            .Where(order => order.Items.Count > 0)
            .ToList();
    }
}
