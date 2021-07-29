namespace NaturalPersonsDirectory.Common
{
    public class Response<TResponse>
    {
        public TResponse Data { get; set; }

        public StatusCode StatusCode { get; set; }

        public string Message { get; set; }
    }
}
