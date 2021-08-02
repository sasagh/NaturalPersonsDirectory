using NaturalPersonsDirectory.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NaturalPersonsDirectory.DAL
{
    public interface IRelationRepository : IRepository<Relation>
    {
        Task<ICollection<Relation>> GetAllWithPaginationAsync(int skip, int take, bool orderByDescending);
        
        Task<bool> RelationWithGivenIdsExistAsync(int fromId, int toId);

        Task<ICollection<Relation>> GetNaturalPersonRelationsAsync(int naturalPersonId);

        Task DeleteRangeAsync(IEnumerable<Relation> relations);
    }
}