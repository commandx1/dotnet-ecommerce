using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Common.Dtos;
using FluentValidation;
using MediatR;

namespace Ecommerce.Application.Features.Reviews.Commands;

public sealed record ReplyReviewCommand(Guid ReviewId, Guid VendorId, string ReplyText) : IRequest<ReviewDto>;

public sealed class ReplyReviewCommandValidator : AbstractValidator<ReplyReviewCommand>
{
    public ReplyReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId).NotEmpty();
        RuleFor(x => x.VendorId).NotEmpty();
        RuleFor(x => x.ReplyText).NotEmpty().MaximumLength(1000);
    }
}

public sealed class ReplyReviewCommandHandler : IRequestHandler<ReplyReviewCommand, ReviewDto>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<ReplyReviewCommand> _validator;

    public ReplyReviewCommandHandler(
        IReviewRepository reviewRepository,
        IUnitOfWork unitOfWork,
        IValidator<ReplyReviewCommand> validator)
    {
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ReviewDto> Handle(ReplyReviewCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var review = await _reviewRepository.GetByIdForVendorAsync(request.ReviewId, request.VendorId, cancellationToken);
        if (review is null)
        {
            throw new KeyNotFoundException("Review not found for vendor.");
        }

        review.ReplyText = request.ReplyText.Trim();
        _reviewRepository.Update(review);
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
