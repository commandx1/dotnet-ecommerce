using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Common.Dtos;
using Ecommerce.Application.Features.Products;
using MediatR;

namespace Ecommerce.Application.Features.Products.Queries;

public sealed record GetPublicProductsQuery : IRequest<IReadOnlyList<ProductDto>>;

public sealed class GetPublicProductsQueryHandler : IRequestHandler<GetPublicProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetPublicProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<ProductDto>> Handle(GetPublicProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetPublicProductsAsync(cancellationToken);
        return products.Select(product => product.ToDto()).ToList();
    }
}
