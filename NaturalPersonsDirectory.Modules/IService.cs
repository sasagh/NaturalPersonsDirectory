using System.Threading.Tasks;

namespace NaturalPersonsDirectory.Modules
{
    public interface IService<TRequest, TResponse>
    {
        Task<TResponse> Create(TRequest model);

        Task<TResponse> GetAll(PaginationParameters parameters);

        Task<TResponse> GetById(int id);

        Task<TResponse> Update(int id, TRequest model);

        Task<TResponse> Delete(int id);
    }
}
