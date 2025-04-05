using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Adoroid.Core.Application.Exceptions.ExceptionDetails;

public class InternalServerExceptionDetail : ProblemDetails
{
    public InternalServerExceptionDetail(string detail)
    {
        Title = "Internal Server Error";
        Detail = detail;
        Status = StatusCodes.Status500InternalServerError;
        Type = "";
    }
}
