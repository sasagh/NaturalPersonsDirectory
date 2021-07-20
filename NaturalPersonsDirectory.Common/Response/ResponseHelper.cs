namespace NaturalPersonsDirectory.Common
{
    public static class ResponseHelper<T> where T : class
    {
        public static Response<T> GetResponse(StatusCode statusCode, T data = null)
        {
            var result = new Response<T>()
            {
                StatusCode = statusCode,
                Message = StatusMessages.GetMessageByStatusCode(statusCode),
                Data = data
            };

            return result;
        }
    }
}
