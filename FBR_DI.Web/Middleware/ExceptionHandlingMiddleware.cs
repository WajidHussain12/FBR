using FBR_DI.Application.Exceptions;
using System.Text.Json;

namespace FBR_DI.Web.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions _jsonOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation error on {Path}: {Errors}",
                context.Request.Path,
                string.Join("; ", ex.Errors.SelectMany(e => e.Value)));

            if (IsAjax(context))
            {
                context.Response.StatusCode  = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(
                        new { success = false, errors = ex.Errors }, _jsonOptions));
            }
            else
            {
                context.Items["ValidationErrors"] = ex.Errors;
                context.Response.Redirect("/Admin/Dashboard/Index");
            }
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Not found on {Path}: {Message}", context.Request.Path, ex.Message);

            if (IsAjax(context))
            {
                context.Response.StatusCode  = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(
                        new { success = false, message = ex.Message }, _jsonOptions));
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.Redirect("/Admin/Dashboard/Index");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on {Path}", context.Request.Path);

            if (IsAjax(context))
            {
                context.Response.StatusCode  = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(
                        new { success = false, message = "An internal error occurred. Please try again." },
                        _jsonOptions));
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.Redirect("/Admin/Dashboard/Index");
            }
        }
    }

    private static bool IsAjax(HttpContext context) =>
        context.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
        (context.Request.ContentType?.Contains("application/json") == true);
}
