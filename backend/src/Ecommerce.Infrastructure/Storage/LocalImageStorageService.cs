using Ecommerce.Application.Abstractions.Storage;
using Microsoft.AspNetCore.Hosting;

namespace Ecommerce.Infrastructure.Storage;

public sealed class LocalImageStorageService : IImageStorageService
{
    private const long MaxImageBytes = 5_000_000;

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    private readonly string _assetsDirectory;

    public LocalImageStorageService(IWebHostEnvironment environment)
    {
        var webRoot = environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRoot))
        {
            webRoot = Path.Combine(environment.ContentRootPath, "wwwroot");
        }

        _assetsDirectory = Path.Combine(webRoot, "assets");
        Directory.CreateDirectory(_assetsDirectory);
    }

    public async Task<string> SaveImageAsync(Stream content, string originalFileName, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(originalFileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("Only image files (.jpg, .jpeg, .png, .webp) are allowed.");
        }

        var safeExtension = extension.ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{safeExtension}";
        var filePath = Path.Combine(_assetsDirectory, fileName);
        await using var buffered = new MemoryStream();
        await content.CopyToAsync(buffered, cancellationToken);

        if (buffered.Length == 0 || buffered.Length > MaxImageBytes)
        {
            throw new InvalidOperationException("Image size must be between 1 byte and 5MB.");
        }

        buffered.Position = 0;

        await using var output = File.Create(filePath);
        await buffered.CopyToAsync(output, cancellationToken);

        return $"/assets/{fileName}";
    }
}
