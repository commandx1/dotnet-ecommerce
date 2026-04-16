using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Common.Dtos;
using Ecommerce.Application.Features.Products;
using MediatR;

namespace Ecommerce.Application.Features.Products.Queries;

public sealed record GetVendorProductsQuery(Guid VendorId) : IRequest<IReadOnlyList<ProductDto>>;

public sealed class GetVendorProductsQueryHandler : IRequestHandler<GetVendorProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetVendorProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<ProductDto>> Handle(GetVendorProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetByVendorAsync(request.VendorId, cancellationToken);
        return products.Select(product => product.ToDto()).ToList();
    }
}
