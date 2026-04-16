using Ecommerce.Api.Common;
using Ecommerce.Api.Contracts;
using Ecommerce.Application.Features.Questions.Commands;
using Ecommerce.Application.Features.Questions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/products/{productId:guid}/questions")]
public sealed class ProductQuestionsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductQuestionsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductQuestions(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetProductQuestionsQuery(productId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Buyer")]
    public async Task<IActionResult> CreateQuestion(Guid productId, CreateQuestionRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateProductQuestionCommand(productId, User.GetRequiredUserId(), request.QuestionText);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }
}
