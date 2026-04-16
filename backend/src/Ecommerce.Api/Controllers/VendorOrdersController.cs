using Ecommerce.Api.Common;
using Ecommerce.Application.Features.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Authorize(Roles = "Vendor")]
[Route("api/vendor/orders")]
public sealed class VendorOrdersController : ControllerBase
{
    private readonly ISender _sender;

    public VendorOrdersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetVendorOrders(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetVendorOrdersQuery(User.GetRequiredUserId()), cancellationToken);
        return Ok(result);
    }
}
