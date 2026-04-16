using Ecommerce.Domain.Questions;

namespace Ecommerce.Application.Abstractions.Persistence;

public interface IQuestionRepository : IRepository<Question>
{
    Task<IReadOnlyList<Question>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Question>> GetByVendorAsync(Guid vendorId, CancellationToken cancellationToken = default);
    Task<Question?> GetByIdForVendorAsync(Guid questionId, Guid vendorId, CancellationToken cancellationToken = default);
}
