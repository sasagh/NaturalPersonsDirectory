using System.Collections.Generic;
using System.Threading.Tasks;

namespace NaturalPersonsDirectory.DAL
{
    public interface IRepository<T>
    {
        Task<T> CreateAsync(T model);

        Task<ICollection<T>> GetAllAsync();

        Task<ICollection<T>> GetAllWithPaginationAsync(int skip, int take);

        Task<T> GetByIdAsync(int id);
        
        Task<bool> ExistsAsync(int id);

        Task<T> UpdateAsync(T model);

        Task DeleteAsync(T model);
    }
}