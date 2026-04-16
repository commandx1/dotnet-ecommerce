using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Common.Dtos;
using Ecommerce.Domain.Reviews;
using FluentValidation;
using MediatR;

namespace Ecommerce.Application.Features.Reviews.Commands;

public sealed record CreateProductReviewCommand(Guid ProductId, Guid BuyerId, int Rating, string Comment) : IRequest<ReviewDto>;

public sealed class CreateProductReviewCommandValidator : AbstractValidator<CreateProductReviewCommand>
{
    public CreateProductReviewCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.BuyerId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).NotEmpty().MaximumLength(1000);
    }
}

public sealed class CreateProductReviewCommandHandler : IRequestHandler<CreateProductReviewCommand, ReviewDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateProductReviewCommand> _validator;

    public CreateProductReviewCommandHandler(
        IProductRepository productRepository,
        IReviewRepository reviewRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateProductReviewCommand> validator)
    {
        _productRepository = productRepository;
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ReviewDto> Handle(CreateProductReviewCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        var review = new Review
        {
            ProductId = request.ProductId,
            BuyerId = request.BuyerId,
            Rating = request.Rating,
            Comment = request.Comment.Trim()
        };

        await _reviewRepository.AddAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ReviewDto(
            review.Id,
            review.ProductId,
            review.BuyerId,
            review.Rating,
            review.Comment,
            review.ReplyText,
            review.CreatedAt);
    }
}
