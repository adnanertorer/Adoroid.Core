using Adoroid.Core.Application.Exceptions.Models;

namespace Adoroid.Core.Application.Exceptions.Types;

public class ValidationException : Exception
{
    public IEnumerable<ValidationExceptionModel> Errors { get; }

    public ValidationException():base()
    {
        Errors = [];
    }

    public ValidationException(string? message): base(message)
    {
        Errors = [];
    }

    public ValidationException(string? message, Exception? innerException)
            : base(message, innerException)
    {
        Errors = [];
    }

    public ValidationException(IEnumerable<ValidationExceptionModel> errors): base(BuildExceptionMessages(errors))
    {
        Errors = errors;
    }

    public static string BuildExceptionMessages(IEnumerable<ValidationExceptionModel> errors)
    {
        IEnumerable<string> exceptionArray = errors.Select(
                x => $"{Environment.NewLine} -- {x.Property}: {string.Join(Environment.NewLine, values: x.Errors ?? [])}"
            );
        return $"Validation failed: {string.Join(string.Empty, exceptionArray)}";
    }
}
