namespace Ecommerce.Api.Common;

public sealed record ApiErrorResponse(
    string Message,
    IReadOnlyList<string>? Errors = null,
    string? TraceId = null);
