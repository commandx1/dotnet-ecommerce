using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Common.Dtos;
using Ecommerce.Domain.Questions;
using FluentValidation;
using MediatR;

namespace Ecommerce.Application.Features.Questions.Commands;

public sealed record CreateProductQuestionCommand(Guid ProductId, Guid BuyerId, string QuestionText) : IRequest<QuestionDto>;

public sealed class CreateProductQuestionCommandValidator : AbstractValidator<CreateProductQuestionCommand>
{
    public CreateProductQuestionCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.BuyerId).NotEmpty();
        RuleFor(x => x.QuestionText).NotEmpty().MaximumLength(1000);
    }
}

public sealed class CreateProductQuestionCommandHandler : IRequestHandler<CreateProductQuestionCommand, QuestionDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateProductQuestionCommand> _validator;

    public CreateProductQuestionCommandHandler(
        IProductRepository productRepository,
        IQuestionRepository questionRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateProductQuestionCommand> validator)
    {
        _productRepository = productRepository;
        _questionRepository = questionRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<QuestionDto> Handle(CreateProductQuestionCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        var question = new Question
        {
            ProductId = request.ProductId,
            BuyerId = request.BuyerId,
            QuestionText = request.QuestionText.Trim()
        };

        await _questionRepository.AddAsync(question, cancellationToken);
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
