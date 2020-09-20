using NaturalPersonsDirectory.Common;
using System.Threading.Tasks;

namespace NaturalPersonsDirectory.Modules
{
    public interface IRelationService : IService<RelationRequest, IResponse<RelationResponse>>
    {
    }
}
