using Microsoft.AspNetCore.Mvc;

namespace Adoroid.Core.Application.Exceptions.ExceptionDetails;

public class CustomExceptionDetail : ProblemDetails
{
    public CustomExceptionDetail(int statusCode, string title, string problemDetailStr)
    {
        Title = title;
        Detail = problemDetailStr;
        Status = statusCode;
        Type = "custom";
    }
}
