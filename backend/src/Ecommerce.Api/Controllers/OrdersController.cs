using Ecommerce.Api.Common;
using Ecommerce.Api.Contracts;
using Ecommerce.Application.Features.Orders.Commands;
using Ecommerce.Application.Features.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Authorize(Roles = "Buyer")]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly ISender _sender;

    public OrdersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CheckoutRequest request, CancellationToken cancellationToken)
    {
        var command = new CheckoutOrderCommand(
            User.GetRequiredUserId(),
            request.Items.Select(item => new CheckoutOrderItem(item.ProductId, item.Quantity)).ToList());

        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetBuyerOrdersQuery(User.GetRequiredUserId()), cancellationToken);
        return Ok(result);
    }
}
