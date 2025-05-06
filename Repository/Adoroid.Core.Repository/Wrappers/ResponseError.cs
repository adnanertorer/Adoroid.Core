using System.Net;
using System.Text.Json;
using Adoroid.Core.Repository.Models;

namespace Adoroid.Core.Repository.Wrappers;

public static class ResponseError
{
    public static async Task<Response<T>> ErrorResponseAsync<T>(
        HttpResponseMessage responseMessage,
        CancellationToken cancellationToken)
    {
        var responseResult = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

        if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
        {
            var badRequestError = JsonSerializer.Deserialize<BadRequestErrorResponseModel>(responseResult);
            if (badRequestError is not null)
            {
                return Response<T>.Fail(badRequestError.Error, statusCode: (int)responseMessage.StatusCode);
            }
        }

        var errorResponse = JsonSerializer.Deserialize<ErrorResponseModel>(responseResult);
        if (errorResponse is not null)
        {
            return Response<T>.Fail(errorResponse.ErrorMessage, statusCode: (int)responseMessage.StatusCode);
        }

        return Response<T>.Fail("Unknown error", statusCode: (int)responseMessage.StatusCode);
    }
}