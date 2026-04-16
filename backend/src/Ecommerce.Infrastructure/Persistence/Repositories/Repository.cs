using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : BaseAuditableEntity
{
    protected readonly ApplicationDbContext DbContext;
    protected readonly DbSet<T> DbSet;

    protected Repository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<T>();
    }

    public virtual Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public virtual IQueryable<T> Query()
    {
        return DbSet.AsQueryable();
    }

    public virtual Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        return DbSet.AddAsync(entity, cancellationToken).AsTask();
    }

    public virtual void Update(T entity)
    {
        DbSet.Update(entity);
    }
}
