using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Common.Dtos;
using Ecommerce.Application.Features.Products;
using FluentValidation;
using MediatR;

namespace Ecommerce.Application.Features.Products.Commands;

public sealed record UpdateVendorProductCommand(
    Guid VendorId,
    Guid ProductId,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string? ImageUrl) : IRequest<ProductDto>;

public sealed class UpdateVendorProductCommandValidator : AbstractValidator<UpdateVendorProductCommand>
{
    public UpdateVendorProductCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.VendorId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdateVendorProductCommandHandler : IRequestHandler<UpdateVendorProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateVendorProductCommand> _validator;

    public UpdateVendorProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateVendorProductCommand> validator)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ProductDto> Handle(UpdateVendorProductCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var entity = await _productRepository.GetByIdForVendorAsync(request.ProductId, request.VendorId, cancellationToken);
        if (entity is null)
        {
            throw new KeyNotFoundException("Product not found for vendor.");
        }

        entity.Name = request.Name.Trim();
        entity.Description = request.Description.Trim();
        entity.Price = request.Price;
        entity.Stock = request.Stock;
        if (!string.IsNullOrWhiteSpace(request.ImageUrl))
        {
            entity.ImageUrl = request.ImageUrl;
        }

        _productRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity.ToDto();
    }
}
