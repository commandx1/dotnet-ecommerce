using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Application.Common.Dtos;
using Ecommerce.Application.Features.Products;
using Ecommerce.Domain.Catalog;
using FluentValidation;
using MediatR;

namespace Ecommerce.Application.Features.Products.Commands;

public sealed record CreateVendorProductCommand(
    Guid VendorId,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string? ImageUrl) : IRequest<ProductDto>;

public sealed class CreateVendorProductCommandValidator : AbstractValidator<CreateVendorProductCommand>
{
    public CreateVendorProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
        RuleFor(x => x.VendorId).NotEmpty();
    }
}

public sealed class CreateVendorProductCommandHandler : IRequestHandler<CreateVendorProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateVendorProductCommand> _validator;

    public CreateVendorProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateVendorProductCommand> validator)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ProductDto> Handle(CreateVendorProductCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var entity = new Product
        {
            VendorId = request.VendorId,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Price = request.Price,
            Stock = request.Stock,
            ImageUrl = request.ImageUrl
        };

        await _productRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity.ToDto();
    }
}
