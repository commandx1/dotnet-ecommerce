using Ecommerce.Domain.Common;

namespace Ecommerce.Domain.Questions;

public sealed class Question : BaseAuditableEntity
{
    public Guid ProductId { get; set; }
    public Guid BuyerId { get; set; }
    public required string QuestionText { get; set; }
    public string? AnswerText { get; set; }
    public DateTimeOffset? AnsweredAt { get; set; }
}
