using Microsoft.AspNetCore.Http;
using NaturalPersonsDirectory.Common;
using System.Threading.Tasks;

namespace NaturalPersonsDirectory.Modules
{
    public interface INaturalPersonService : IService<NaturalPersonRequest, IResponse<NaturalPersonResponse>>
    {
        Task<IResponse<RelatedPersonsResponse>> GetRelatedPersons(int id);
        Task<IResponse<NaturalPersonResponse>> AddImage(int id, IFormFile image);
        Task<IResponse<NaturalPersonResponse>> UpdateImage(int id, IFormFile image);
        Task<IResponse<NaturalPersonResponse>> DeleteImage(int id);
    }
}
