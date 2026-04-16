using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Common.Dtos;
using MediatR;

namespace Ecommerce.Application.Features.Questions.Queries;

public sealed record GetVendorQuestionsQuery(Guid VendorId) : IRequest<IReadOnlyList<QuestionDto>>;

public sealed class GetVendorQuestionsQueryHandler : IRequestHandler<GetVendorQuestionsQuery, IReadOnlyList<QuestionDto>>
{
    private readonly IQuestionRepository _questionRepository;

    public GetVendorQuestionsQueryHandler(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task<IReadOnlyList<QuestionDto>> Handle(GetVendorQuestionsQuery request, CancellationToken cancellationToken)
    {
        var questions = await _questionRepository.GetByVendorAsync(request.VendorId, cancellationToken);

        return questions
            .Select(question => new QuestionDto(
                question.Id,
                question.ProductId,
                question.BuyerId,
                question.QuestionText,
                question.AnswerText,
                question.CreatedAt,
                question.AnsweredAt))
            .ToList();
    }
}
