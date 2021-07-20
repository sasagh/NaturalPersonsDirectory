using Microsoft.AspNetCore.Http;
using NaturalPersonsDirectory.Common;
using System.Threading.Tasks;

namespace NaturalPersonsDirectory.Modules
{
    public interface INaturalPersonService : IService<NaturalPersonRequest, Response<NaturalPersonResponse>>
    {
        Task<Response<RelatedPersonsResponse>> GetRelatedPersons(int id);
        Task<Response<NaturalPersonResponse>> AddImage(int id, IFormFile image);
        Task<Response<NaturalPersonResponse>> UpdateImage(int id, IFormFile image);
        Task<Response<NaturalPersonResponse>> DeleteImage(int id);
    }
}
