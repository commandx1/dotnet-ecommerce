using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Common.Dtos;
using Ecommerce.Domain.Orders;
using FluentValidation;
using MediatR;

namespace Ecommerce.Application.Features.Orders.Commands;

public sealed record CheckoutOrderItem(Guid ProductId, int Quantity);

public sealed record CheckoutOrderCommand(Guid BuyerId, IReadOnlyList<CheckoutOrderItem> Items)
    : IRequest<CheckoutOrderResultDto>;

public sealed class CheckoutOrderCommandValidator : AbstractValidator<CheckoutOrderCommand>
{
    public CheckoutOrderCommandValidator()
    {
        RuleFor(x => x.BuyerId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.ProductId).NotEmpty();
                item.RuleFor(x => x.Quantity).GreaterThan(0);
            });
    }
}

public sealed class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, CheckoutOrderResultDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CheckoutOrderCommand> _validator;

    public CheckoutOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IValidator<CheckoutOrderCommand> validator)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CheckoutOrderResultDto> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var groupedItems = request.Items
            .GroupBy(x => x.ProductId)
            .Select(group => new CheckoutOrderItem(group.Key, group.Sum(x => x.Quantity)))
            .ToList();

        var productIds = groupedItems.Select(x => x.ProductId).ToArray();
        var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

        if (products.Count != productIds.Length)
        {
            throw new KeyNotFoundException("One or more products could not be found.");
        }

        var productMap = products.ToDictionary(x => x.Id);

        var orderItems = new List<OrderItem>();
        decimal totalAmount = 0;

        foreach (var item in groupedItems)
        {
            var product = productMap[item.ProductId];

            if (product.Stock < item.Quantity)
            {
                throw new ValidationException($"Insufficient stock for product: {product.Name}");
            }

            product.Stock -= item.Quantity;
            _productRepository.Update(product);

            orderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });

            totalAmount += product.Price * item.Quantity;
        }

        var order = new Order
        {
            BuyerId = request.BuyerId,
            Status = OrderStatus.Pending,
            TotalAmount = totalAmount,
            Items = orderItems
        };

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CheckoutOrderResultDto(order.Id, order.TotalAmount, order.Status);
    }
}
