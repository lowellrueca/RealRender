using System.Net;

namespace RealRender.ProductApiService.Exceptions;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            await WriteExceptionAsync(exception, response);
        }
    }

    async Task WriteExceptionAsync(Exception exception, HttpResponse response)
    {
        switch (exception)
        {
            case ApplicationException exc: // custom application error
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case KeyNotFoundException exc:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
            default: // unhandled error
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        _logger.LogError($"An exception occured unexpectedly, {exception}");
        await response.WriteAsync(new ExceptionDetail { StatusCode = response.StatusCode, Message = exception?.Message }.ToString());
    }
}
