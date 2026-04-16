using Ecommerce.Api.Common;
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
            await WriteErrorAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Validation error.",
                validationException.Errors.Select(x => x.ErrorMessage).ToArray());
        }
        catch (BadHttpRequestException badHttpRequestException)
        {
            _logger.LogWarning(badHttpRequestException, "Invalid HTTP request.");
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, badHttpRequestException.Message);
        }
        catch (InvalidOperationException invalidOperationException)
        {
            _logger.LogWarning(invalidOperationException, "Invalid operation rejected.");
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, invalidOperationException.Message);
        }
        catch (UnauthorizedAccessException unauthorizedException)
        {
            _logger.LogWarning(unauthorizedException, "Unauthorized request blocked.");
            await WriteErrorAsync(context, StatusCodes.Status401Unauthorized, "Unauthorized request.");
        }
        catch (KeyNotFoundException keyNotFoundException)
        {
            _logger.LogWarning(keyNotFoundException, "Requested resource not found.");
            await WriteErrorAsync(context, StatusCodes.Status404NotFound, "Requested resource was not found.");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception occurred.");
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    private static Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string message,
        IReadOnlyList<string>? errors = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var payload = new ApiErrorResponse(message, errors, context.TraceIdentifier);
        return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
