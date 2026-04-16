using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Common.Dtos;
using MediatR;

namespace Ecommerce.Application.Features.Reviews.Queries;

public sealed record GetProductReviewsQuery(Guid ProductId) : IRequest<IReadOnlyList<ReviewDto>>;

public sealed class GetProductReviewsQueryHandler : IRequestHandler<GetProductReviewsQuery, IReadOnlyList<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetProductReviewsQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IReadOnlyList<ReviewDto>> Handle(GetProductReviewsQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetByProductAsync(request.ProductId, cancellationToken);

        return reviews
            .Select(review => new ReviewDto(
                review.Id,
                review.ProductId,
                review.BuyerId,
                review.Rating,
                review.Comment,
                review.ReplyText,
                review.CreatedAt))
            .ToList();
    }
}
