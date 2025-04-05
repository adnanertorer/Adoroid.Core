namespace Adoroid.Core.Application.Exceptions.Models;

public class ValidationExceptionModel
{
    public string? Property { get; set; }
    public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
}
