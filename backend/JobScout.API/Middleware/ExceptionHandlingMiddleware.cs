using System.Net;
using System.Text.Json;
using JobScout.Application.DTOs;
using JobScout.Application.Exceptions;

namespace JobScout.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (JobScoutException ex)
        {
            _logger.LogInformation(ex, "Handled JobScout error {Code}", ex.Code);
            await WriteError(context, ex.StatusCode, ex.Code, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteError(
                context,
                (int)HttpStatusCode.InternalServerError,
                "server_error",
                "Something went wrong. Please try again.");
        }
    }

    private static async Task WriteError(HttpContext context, int statusCode, string code, string message)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var payload = new ApiErrorResponse
        {
            Error = new ApiError
            {
                Code = code,
                Message = message
            }
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
