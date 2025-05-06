using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Adoroid.Core.Application.Models;

namespace Adoroid.Core.Application.Wrappers;

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