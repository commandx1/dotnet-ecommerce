using Ecommerce.Domain.Common;

namespace Ecommerce.Application.Abstractions.Persistence;

public interface IRepository<T> where T : BaseAuditableEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    IQueryable<T> Query();
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
}
