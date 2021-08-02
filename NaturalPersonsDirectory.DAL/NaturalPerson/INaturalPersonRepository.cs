using Microsoft.AspNetCore.Http;
using NaturalPersonsDirectory.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NaturalPersonsDirectory.DAL
{
    public interface INaturalPersonRepository : IRepository<NaturalPerson>
    {
        Task<ICollection<NaturalPerson>> GetAllWithPaginationAsync(
            int skip,
            int take,
            bool orderByDescending,
            string orderBy);
        
        Task<NaturalPerson> GetByPassportNumberAsync(string passportNumber);

        Task<ICollection<RelatedPerson>> GetRelatedPersonsAsync(int naturalPersonId);

        Task<string> UploadImageAsync(IFormFile image);
    }
}