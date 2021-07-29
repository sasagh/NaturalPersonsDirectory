using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NaturalPersonsDirectory.Models;

namespace NaturalPersonsDirectory.DAL
{
    public interface IRelationRepository : IRepository<Relation>
    {
        Task<IEnumerable<Relation>> GetNaturalPersonRelationsAsync(int naturalPersonId);
        
        Task DeleteRangeAsync(IEnumerable<Relation> relations);
    }
}