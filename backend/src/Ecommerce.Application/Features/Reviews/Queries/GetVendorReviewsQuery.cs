using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Common.Dtos;
using MediatR;

namespace Ecommerce.Application.Features.Reviews.Queries;

public sealed record GetVendorReviewsQuery(Guid VendorId) : IRequest<IReadOnlyList<ReviewDto>>;

public sealed class GetVendorReviewsQueryHandler : IRequestHandler<GetVendorReviewsQuery, IReadOnlyList<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetVendorReviewsQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IReadOnlyList<ReviewDto>> Handle(GetVendorReviewsQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetByVendorAsync(request.VendorId, cancellationToken);

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
