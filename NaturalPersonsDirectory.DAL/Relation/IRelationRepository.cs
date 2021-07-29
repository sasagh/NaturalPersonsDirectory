﻿using NaturalPersonsDirectory.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NaturalPersonsDirectory.DAL
{
    public interface IRelationRepository : IRepository<Relation>
    {
        Task<bool> RelationWithGivenIdsExist(int fromId, int toId);

        Task<ICollection<Relation>> GetNaturalPersonRelationsAsync(int naturalPersonId);

        Task DeleteRangeAsync(IEnumerable<Relation> relations);
    }
}