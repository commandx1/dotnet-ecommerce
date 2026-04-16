namespace Ecommerce.Application.Common.Dtos;

public sealed record QuestionDto(
    Guid Id,
    Guid ProductId,
    Guid BuyerId,
    string QuestionText,
    string? AnswerText,
    DateTimeOffset CreatedAt,
    DateTimeOffset? AnsweredAt);
