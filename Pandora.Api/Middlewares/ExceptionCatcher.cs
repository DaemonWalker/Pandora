using System.Net;
using System.Text.Json;
using Pandora.Api.Model;

namespace Pandora.Api.Middlewares;

public class ExceptionCatcherMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (LogicException ex)
        {
            await HandleLogicExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleLogicExceptionAsync(HttpContext context, LogicException ex)
    {
        var code = HttpStatusCode.BadRequest;
        return HandleExceptionAsync(context, ex, code);
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode code = HttpStatusCode.InternalServerError)
    {
        var result = JsonSerializer.Serialize(new { error = ex.Message });
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}
