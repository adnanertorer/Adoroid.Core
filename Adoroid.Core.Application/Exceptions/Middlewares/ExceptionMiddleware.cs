using Adoroid.Core.Application.Exceptions.Handlers;
using Microsoft.AspNetCore.Http;

namespace Adoroid.Core.Application.Exceptions.Middlewares;

public class ExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    private readonly HttpExceptionHandler _exceptionHandler = new();

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleException(context.Response, exception);
        }
    }

    private Task HandleException(HttpResponse response, Exception exception)
    {
       response.ContentType = "application/json";
        _exceptionHandler.Response = response;
        return _exceptionHandler.HandleExceptionAsync(exception);
    }
}
