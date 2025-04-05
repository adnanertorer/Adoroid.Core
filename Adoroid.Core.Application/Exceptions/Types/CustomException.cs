using System.Net;

namespace Adoroid.Core.Application.Exceptions.Types;

public class CustomException(HttpStatusCode statusCode, string title, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
    public string Title { get; } = title;
}
