using Ecommerce.Application.Abstractions.Persistence;
using FluentValidation;
using MediatR;

namespace Ecommerce.Application.Features.Products.Commands;

public sealed record DeleteVendorProductCommand(Guid VendorId, Guid ProductId) : IRequest;

public sealed class DeleteVendorProductCommandValidator : AbstractValidator<DeleteVendorProductCommand>
{
    public DeleteVendorProductCommandValidator()
    {
        RuleFor(x => x.VendorId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
    }
}

public sealed class DeleteVendorProductCommandHandler : IRequestHandler<DeleteVendorProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteVendorProductCommand> _validator;

    public DeleteVendorProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IValidator<DeleteVendorProductCommand> validator)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task Handle(DeleteVendorProductCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var entity = await _productRepository.GetByIdForVendorAsync(request.ProductId, request.VendorId, cancellationToken);
        if (entity is null)
        {
            throw new KeyNotFoundException("Product not found for vendor.");
        }

        entity.IsDeleted = true;
        _productRepository.Update(entity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
