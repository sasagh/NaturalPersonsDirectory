using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NaturalPersonsDirectory.DAL
{
    public interface IRepository<T>
    {
        Task<T> CreateAsync(T model);
        
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(int id);
        
        Task<T> UpdateAsync(T model);
        
        Task DeleteAsync(T model);
    }
}