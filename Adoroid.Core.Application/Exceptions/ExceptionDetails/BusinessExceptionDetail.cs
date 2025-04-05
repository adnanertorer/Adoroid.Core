using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Adoroid.Core.Application.Exceptions.ExceptionDetails;

public class BusinessExceptionDetail : ProblemDetails
{
    public BusinessExceptionDetail(string detail)
    {
        Title = "Rule";
        Detail = detail;
        Status = StatusCodes.Status400BadRequest;
        Type = "";
    }
}
