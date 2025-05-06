namespace Adoroid.Core.Repository.Models;

internal class ErrorResponseModel
{
    public string Field { get; set; } = null!;
    public string ErrorMessage { get; set; } = null!;
}