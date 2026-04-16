using Ecommerce.Api.Common;
using Ecommerce.Api.Contracts;
using Ecommerce.Application.Abstractions.Storage;
using Ecommerce.Application.Features.Products.Commands;
using Ecommerce.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Authorize(Roles = "Vendor")]
[Route("api/vendor/products")]
public sealed class VendorProductsController : ControllerBase
{
    private const long MaxImageBytes = 5_000_000;
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    private readonly ISender _sender;
    private readonly IImageStorageService _imageStorageService;

    public VendorProductsController(ISender sender, IImageStorageService imageStorageService)
    {
        _sender = sender;
        _imageStorageService = imageStorageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetVendorProducts(CancellationToken cancellationToken)
    {
        var vendorId = User.GetRequiredUserId();
        var result = await _sender.Send(new GetVendorProductsQuery(vendorId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> Create([FromForm] CreateVendorProductRequest request, CancellationToken cancellationToken)
    {
        var imageUrl = await SaveImageIfUploadedAsync(request.Image, cancellationToken);
        var command = new CreateVendorProductCommand(
            User.GetRequiredUserId(),
            request.Name,
            request.Description,
            request.Price,
            request.Stock,
            imageUrl);

        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(CatalogController.GetProductById), "Catalog", new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateVendorProductRequest request, CancellationToken cancellationToken)
    {
        var imageUrl = await SaveImageIfUploadedAsync(request.Image, cancellationToken);

        var command = new UpdateVendorProductCommand(
            User.GetRequiredUserId(),
            id,
            request.Name,
            request.Description,
            request.Price,
            request.Stock,
            imageUrl);

        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteVendorProductCommand(User.GetRequiredUserId(), id), cancellationToken);
        return NoContent();
    }

    private async Task<string?> SaveImageIfUploadedAsync(IFormFile? image, CancellationToken cancellationToken)
    {
        if (image is null || image.Length == 0)
        {
            return null;
        }

        if (image.Length > MaxImageBytes)
        {
            throw new BadHttpRequestException("Image size must be 5MB or less.");
        }

        if (string.IsNullOrWhiteSpace(image.ContentType) || !AllowedContentTypes.Contains(image.ContentType))
        {
            throw new BadHttpRequestException("Only image/jpeg, image/png, and image/webp are allowed.");
        }

        await using var stream = image.OpenReadStream();
        return await _imageStorageService.SaveImageAsync(stream, image.FileName, cancellationToken);
    }
}
