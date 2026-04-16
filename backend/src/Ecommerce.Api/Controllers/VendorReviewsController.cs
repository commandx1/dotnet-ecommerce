using Ecommerce.Api.Common;
using Ecommerce.Api.Contracts;
using Ecommerce.Application.Features.Reviews.Commands;
using Ecommerce.Application.Features.Reviews.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Authorize(Roles = "Vendor")]
[Route("api/vendor/reviews")]
public sealed class VendorReviewsController : ControllerBase
{
    private readonly ISender _sender;

    public VendorReviewsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetVendorReviews(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetVendorReviewsQuery(User.GetRequiredUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{reviewId:guid}/reply")]
    public async Task<IActionResult> Reply(Guid reviewId, ReplyReviewRequest request, CancellationToken cancellationToken)
    {
        var command = new ReplyReviewCommand(reviewId, User.GetRequiredUserId(), request.ReplyText);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }
}
