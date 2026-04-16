using Ecommerce.Application.Abstractions.Persistence;
using Ecommerce.Domain.Questions;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Persistence.Repositories;

public sealed class QuestionRepository : Repository<Question>, IQuestionRepository
{
    public QuestionRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Question>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .AsNoTracking()
            .Where(question => question.ProductId == productId)
            .OrderByDescending(question => question.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Question>> GetByVendorAsync(Guid vendorId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .AsNoTracking()
            .Where(question => DbContext.Products.Any(product => product.Id == question.ProductId && product.VendorId == vendorId))
            .OrderByDescending(question => question.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<Question?> GetByIdForVendorAsync(Guid questionId, Guid vendorId, CancellationToken cancellationToken = default)
    {
        return Query().FirstOrDefaultAsync(
            question => question.Id == questionId
                && DbContext.Products.Any(product => product.Id == question.ProductId && product.VendorId == vendorId),
            cancellationToken);
    }
}
