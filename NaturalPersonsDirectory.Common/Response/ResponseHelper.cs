namespace NaturalPersonsDirectory.Common
{
    public static class ResponseHelper
    {
        public static IResponse<TResponse> Ok<TResponse>(TResponse data) where TResponse : class
        {
            var result = new ResponseService<TResponse>()
            {
                Status = Statuses.GetStatus[StatusCode.Success],
                Data = data
            };

            return result;
        }

        public static IResponse<TResponse> Ok<TResponse>(TResponse data, StatusCode statusCode) where TResponse : class
        {
            var result = new ResponseService<TResponse>()
            {
                Status = Statuses.GetStatus[statusCode],
                Data = data
            };

            return result;
        }

        public static IResponse<TResponse> NotFound<TResponse>(TResponse data) where TResponse : class
        {
            var result = new ResponseService<TResponse>()
            {
                Status = Statuses.GetStatus[StatusCode.NotFound],
                Data = data
            };

            return result;
        }

        public static IResponse<TResponse> Fail<TResponse>() where TResponse : class
        {
            var result = new ResponseService<TResponse>()
            {
                Status = Statuses.GetStatus[StatusCode.Error],
                Data = default
            };

            return result;
        }

        public static IResponse<TResponse> Fail<TResponse>(StatusCode statusCode) where TResponse : class
        {
            var result = new ResponseService<TResponse>()
            {
                Status = Statuses.GetStatus[statusCode],
                Data = default
            };

            return result;
        }
    }
}
