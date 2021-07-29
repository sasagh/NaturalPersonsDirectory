using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NaturalPersonsDirectory.Models;

namespace NaturalPersonsDirectory.DAL
{
    public interface INaturalPersonRepository : IRepository<NaturalPerson>
    {
        Task<IEnumerable<NaturalPerson>> GetAllWithParametersAsync(int skip, int take);
        
        Task<NaturalPerson> GetByPassportNumberAsync(string passportNumber);
        
        Task<IEnumerable<RelatedPerson>> GetRelatedPersonsAsync(int naturalPersonId);
        
        Task<string> UploadImageAsync(IFormFile image);
    }
}