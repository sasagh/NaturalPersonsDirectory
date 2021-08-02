using System.Threading.Tasks;
using NaturalPersonsDirectory.Common;

namespace NaturalPersonsDirectory.Modules
{
    public interface IRelationService : IService<RelationRequest, Response<RelationResponse>>
    {
        Task<Response<RelationResponse>> Get(RelationPaginationParameters parameters);
    }
}
