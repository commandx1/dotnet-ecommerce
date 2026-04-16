using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Common.Dtos;
using MediatR;

namespace Ecommerce.Application.Features.Questions.Queries;

public sealed record GetProductQuestionsQuery(Guid ProductId) : IRequest<IReadOnlyList<QuestionDto>>;

public sealed class GetProductQuestionsQueryHandler : IRequestHandler<GetProductQuestionsQuery, IReadOnlyList<QuestionDto>>
{
    private readonly IQuestionRepository _questionRepository;

    public GetProductQuestionsQueryHandler(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task<IReadOnlyList<QuestionDto>> Handle(GetProductQuestionsQuery request, CancellationToken cancellationToken)
    {
        var questions = await _questionRepository.GetByProductAsync(request.ProductId, cancellationToken);

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
