using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Api.Contracts;

public sealed class CreateQuestionRequest
{
    [Required]
    [MaxLength(1000)]
    public string QuestionText { get; set; } = string.Empty;
}

public sealed class AnswerQuestionRequest
{
    [Required]
    [MaxLength(1000)]
    public string AnswerText { get; set; } = string.Empty;
}
