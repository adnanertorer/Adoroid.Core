using Adoroid.Core.Application.Exceptions.Types;

namespace Adoroid.Core.Application.Exceptions.Handlers;

public abstract class ExceptionHandler
{
    public Task HandleExceptionAsync(Exception exception) =>
        exception switch
        {
            BusinessException businessException => HandleException(businessException),
            ValidationException validationException => HandleException(validationException),
            CustomException customException => HandleCustomException(customException),
            _ => HandleException(exception),
        };

    protected abstract Task HandleException(BusinessException businessException);
    protected abstract Task HandleException(ValidationException validationException);
    protected abstract Task HandleException(Exception exception);
    protected abstract Task HandleCustomException(CustomException customException);
}
