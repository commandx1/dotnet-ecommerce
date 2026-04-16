using FluentValidation;
using System.Text.Json;

namespace Ecommerce.Api.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException validationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                message = "Validation error",
                errors = validationException.Errors.Select(x => x.ErrorMessage)
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
        catch (UnauthorizedAccessException unauthorizedException)
        {
            _logger.LogWarning(unauthorizedException, "Unauthorized request blocked.");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        catch (KeyNotFoundException keyNotFoundException)
        {
            _logger.LogWarning(keyNotFoundException, "Requested resource not found.");
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception occurred.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                message = "An unexpected error occurred."
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
