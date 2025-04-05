using Adoroid.Core.Application.Exceptions.ExceptionDetails;
using Adoroid.Core.Application.Exceptions.Types;
using Adoroid.Core.Application.Exceptions.Extensions;
using Microsoft.AspNetCore.Http;

namespace Adoroid.Core.Application.Exceptions.Handlers;

public class HttpExceptionHandler : ExceptionHandler
{
    private HttpResponse? _response;

    public HttpResponse Response
    {
        get => _response ?? throw new ArgumentNullException(nameof(_response));
        set => _response = value;
    }

    protected override Task HandleException(BusinessException businessException)
    {
        Response.StatusCode = StatusCodes.Status400BadRequest;
        string detail = new BusinessExceptionDetail(businessException.Message).AsJson();
        return Response.WriteAsync(detail);
    }

    protected override Task HandleException(ValidationException validationException)
    {
        Response.StatusCode = StatusCodes.Status400BadRequest;
        string details = new ValidationExceptionDetail(validationException.Errors).AsJson();
        return Response.WriteAsync(details);
    }

    protected override Task HandleException(Exception exception)
    {
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        string details = new InternalServerExceptionDetail(exception.Message).AsJson();
        return Response.WriteAsync(details);
    }

    protected override Task HandleCustomException(CustomException customException)
    {
        Response.StatusCode = (int)customException.StatusCode;
        string details = new CustomExceptionDetail((int)customException.StatusCode, customException.Title, customException.Message).AsJson();
        return Response.WriteAsync(details);
    }
}
