namespace Quizer.Web.Middleware;

/// <summary>
/// Middleware, который разрешает CORS-доступ со всех origins.
/// Используется только в dev-среде.
/// </summary>
public class DisableCorsMiddleware
{
    private readonly RequestDelegate _next;

    public DisableCorsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers["Access-Control-Allow-Origin"] = "*";
        context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, PATCH, DELETE, OPTIONS";
        context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization";

        if (HttpMethods.IsOptions(context.Request.Method))
        {
            context.Response.StatusCode = StatusCodes.Status204NoContent;
            return;
        }

        await _next(context);
    }
}
