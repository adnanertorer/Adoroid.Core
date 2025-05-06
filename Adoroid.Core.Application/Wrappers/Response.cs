namespace Adoroid.Core.Application.Wrappers;

public class Response<T>
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public int? StatusCode { get; set; }

    public static Response<T> Success(T data, string? message = null, int? statusCode = 200)
        => new() { Succeeded = true, Data = data, Message = message, StatusCode = statusCode };

    public static Response<T> Fail(string message, List<string>? errors = null, int? statusCode = 400)
        => new() { Succeeded = false, Message = message, Errors = errors, StatusCode = statusCode };
}