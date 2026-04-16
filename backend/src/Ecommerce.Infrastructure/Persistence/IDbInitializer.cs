namespace Ecommerce.Infrastructure.Persistence;

public interface IDbInitializer
{
    Task MigrateAsync(CancellationToken cancellationToken = default);
}
