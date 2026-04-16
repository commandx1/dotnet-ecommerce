using Ecommerce.Api.Common;
using Ecommerce.Api.Contracts;
using Ecommerce.Application.Features.Reviews.Commands;
using Ecommerce.Application.Features.Reviews.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/products/{productId:guid}/reviews")]
public sealed class ProductReviewsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductReviewsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductReviews(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetProductReviewsQuery(productId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Buyer")]
    public async Task<IActionResult> CreateReview(Guid productId, CreateReviewRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateProductReviewCommand(productId, User.GetRequiredUserId(), request.Rating, request.Comment);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }
}
