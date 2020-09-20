namespace NaturalPersonsDirectory.Common
{
    public class ResponseService<TResponse> : IResponse<TResponse>
    {
        public TResponse Data { get; set; }
        public Status Status { get; set; }
    }
}
