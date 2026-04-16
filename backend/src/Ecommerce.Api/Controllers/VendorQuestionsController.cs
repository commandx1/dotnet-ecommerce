using Ecommerce.Api.Common;
using Ecommerce.Api.Contracts;
using Ecommerce.Application.Features.Questions.Commands;
using Ecommerce.Application.Features.Questions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Authorize(Roles = "Vendor")]
[Route("api/vendor/questions")]
public sealed class VendorQuestionsController : ControllerBase
{
    private readonly ISender _sender;

    public VendorQuestionsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetVendorQuestions(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetVendorQuestionsQuery(User.GetRequiredUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{questionId:guid}/answer")]
    public async Task<IActionResult> Answer(Guid questionId, AnswerQuestionRequest request, CancellationToken cancellationToken)
    {
        var command = new AnswerQuestionCommand(questionId, User.GetRequiredUserId(), request.AnswerText);
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }
}
