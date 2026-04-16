using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Api.Contracts;

public sealed class CreateReviewRequest
{
    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Comment { get; set; } = string.Empty;
}

public sealed class ReplyReviewRequest
{
    [Required]
    [MaxLength(1000)]
    public string ReplyText { get; set; } = string.Empty;
}
