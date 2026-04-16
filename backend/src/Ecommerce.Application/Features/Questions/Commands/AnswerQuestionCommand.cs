using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Common.Dtos;
using FluentValidation;
using MediatR;

namespace Ecommerce.Application.Features.Questions.Commands;

public sealed record AnswerQuestionCommand(Guid QuestionId, Guid VendorId, string AnswerText) : IRequest<QuestionDto>;

public sealed class AnswerQuestionCommandValidator : AbstractValidator<AnswerQuestionCommand>
{
    public AnswerQuestionCommandValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.VendorId).NotEmpty();
        RuleFor(x => x.AnswerText).NotEmpty().MaximumLength(1000);
    }
}

public sealed class AnswerQuestionCommandHandler : IRequestHandler<AnswerQuestionCommand, QuestionDto>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<AnswerQuestionCommand> _validator;

    public AnswerQuestionCommandHandler(
        IQuestionRepository questionRepository,
        IUnitOfWork unitOfWork,
        IValidator<AnswerQuestionCommand> validator)
    {
        _questionRepository = questionRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<QuestionDto> Handle(AnswerQuestionCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var question = await _questionRepository.GetByIdForVendorAsync(request.QuestionId, request.VendorId, cancellationToken);
        if (question is null)
        {
            throw new KeyNotFoundException("Question not found for vendor.");
        }

        question.AnswerText = request.AnswerText.Trim();
        question.AnsweredAt = DateTimeOffset.UtcNow;

        _questionRepository.Update(question);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new QuestionDto(
            question.Id,
            question.ProductId,
            question.BuyerId,
            question.QuestionText,
            question.AnswerText,
            question.CreatedAt,
            question.AnsweredAt);
    }
}
