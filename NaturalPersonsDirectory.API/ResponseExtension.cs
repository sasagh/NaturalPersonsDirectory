using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NaturalPersonsDirectory.Common;

namespace NaturalPersonsDirectory.API
{
    public static class ResponseExtension
    {
        public static ActionResult<T> MatchActionResult<T>(this Response<T> response)
        {
            switch (response.StatusCode)
            {
                case StatusCode.Create:
                    return Success(StatusCodes.Status201Created);
                case StatusCode.Delete:
                    return Success(StatusCodes.Status204NoContent);
                case StatusCode.Success:
                case StatusCode.Update:
                case StatusCode.ImageAdded:
                case StatusCode.ImageUpdated:
                case StatusCode.ImageDeleted:
                    return Success(StatusCodes.Status200OK);
                case StatusCode.NotFound:
                    return Error(StatusCodes.Status404NotFound);
                default:
                    return Error(StatusCodes.Status400BadRequest);
            }

            ObjectResult Success(int statusCode) => new ObjectResult(response.Message)
            {
                Value = response.Data,
                StatusCode = statusCode
            };

            ActionResult Error(int statusCode) => new ContentResult
            {
                StatusCode = statusCode,
                Content = $"Status Code: {statusCode}; {response.Message}",
                ContentType = "text/plain",
            };
        }
    }

}