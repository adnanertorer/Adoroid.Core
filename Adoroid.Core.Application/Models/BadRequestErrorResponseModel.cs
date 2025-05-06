namespace Adoroid.Core.Application.Models;

internal class BadRequestErrorResponseModel
{
    public string Error { get; set; } = null!;
    public string ErrorDescription { get; set; } = null!;
}