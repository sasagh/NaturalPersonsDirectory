namespace NaturalPersonsDirectory.Common
{
    public interface IResponse<TResponse>
    {
        TResponse Data { get; set; }
        Status Status { get; set; }
    }
}
