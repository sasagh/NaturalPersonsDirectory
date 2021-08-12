namespace NaturalPersonsDirectory.Common
{
    public class Response<T>
    {
        public T Data { get; set; }

        public StatusCode StatusCode { get; set; }

        public string Message { get; set; }
    }
}
