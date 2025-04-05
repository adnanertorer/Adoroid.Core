using Adoroid.Core.Application.Exceptions.Models;
using FluentValidation;
using MediatR;
using Types_ValidationException = Adoroid.Core.Application.Exceptions.Types.ValidationException;
using ValidationException = Adoroid.Core.Application.Exceptions.Types.ValidationException;

namespace Adoroid.Core.Application.Pipelines.Validation;

public class RequestValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ValidationContext<object> validationContext = new(request);

        IEnumerable<ValidationExceptionModel> errors = _validators
            .Select(v => v.Validate(validationContext))
            .SelectMany(result => result.Errors)
            .Where(failure => failure != null)
            .GroupBy(
            keySelector: p => p.PropertyName, resultSelector: (propertyName, errors) =>
            new ValidationExceptionModel { Errors = errors.Select(i => i.ErrorMessage), Property = propertyName})
            .ToList();

        if (errors.Any())
            throw new Types_ValidationException(errors);
        TResponse response = await next();
        return response;
    }
}
