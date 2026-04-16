namespace Ecommerce.Application.Abstractions.Storage;

public interface IImageStorageService
{
    Task<string> SaveImageAsync(Stream content, string originalFileName, CancellationToken cancellationToken = default);
}
