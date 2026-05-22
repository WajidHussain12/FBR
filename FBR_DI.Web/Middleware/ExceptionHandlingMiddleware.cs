using FBR_DI.Application.Exceptions;
using System.Text.Json;

namespace FBR_DI.Web.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IConfiguration _configuration;

    private static readonly JsonSerializerOptions _jsonOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
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
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(
                        new { success = false, errors = ex.Errors }, _jsonOptions));
            }
            else
            {
                context.Items["ValidationErrors"] = ex.Errors;
                RedirectToDashboard(context);
            }
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Not found on {Path}: {Message}", context.Request.Path, ex.Message);

            if (IsAjax(context))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(
                        new { success = false, message = ex.Message }, _jsonOptions));
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                RedirectToDashboard(context);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on {Path}", context.Request.Path);

            if (IsAjax(context))
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(
                        new { success = false, message = "An internal error occurred. Please try again." },
                        _jsonOptions));
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                RedirectToDashboard(context);
            }
        }
    }

    private void RedirectToDashboard(HttpContext context)
    {
        var pathBase = GetEffectivePathBase(context);
        context.Response.Redirect($"{pathBase}/Admin/Dashboard/Index");
    }

    private string GetEffectivePathBase(HttpContext context)
    {
        var requestPathBase = context.Request.PathBase.HasValue
            ? context.Request.PathBase.Value
            : string.Empty;
        var configuredPathBase = _configuration["PathBase"] ?? string.Empty;

        var pathBase = !string.IsNullOrWhiteSpace(requestPathBase)
            ? requestPathBase
            : configuredPathBase;

        if (string.IsNullOrWhiteSpace(pathBase))
            return string.Empty;

        pathBase = pathBase.Trim();
        pathBase = pathBase.StartsWith('/') ? pathBase : "/" + pathBase;
        return pathBase.TrimEnd('/');
    }

    private static bool IsAjax(HttpContext context) =>
        context.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
        (context.Request.ContentType?.Contains("application/json") == true);
}
