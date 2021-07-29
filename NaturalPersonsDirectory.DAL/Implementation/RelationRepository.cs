using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NaturalPersonsDirectory.Db;
using NaturalPersonsDirectory.Models;

namespace NaturalPersonsDirectory.DAL
{
    public class RelationRepository : IRelationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public RelationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<Relation> CreateAsync(Relation relation)
        {
            _dbContext.Relations.Add(relation);
            await _dbContext.SaveChangesAsync();

            return relation;
        }

        public async Task<IEnumerable<Relation>> GetAllAsync()
        {
            return await _dbContext.Relations.ToListAsync();
        }

        public async Task<Relation> GetByIdAsync(int id)
        {
            return 
                await _dbContext.Relations.FirstOrDefaultAsync(relation => relation.Id == id);
        }

        public async Task<Relation> UpdateAsync(Relation relation)
        {
            _dbContext.Relations.Update(relation);
            await _dbContext.SaveChangesAsync();

            return relation;
        }

        public async Task DeleteAsync(Relation relation)
        {
            _dbContext.Relations.Remove(relation);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Relation>> GetNaturalPersonRelationsAsync(int naturalPersonId)
        {
            return await _dbContext
                .Relations
                .Where(relation => relation.FromId == naturalPersonId || relation.ToId == naturalPersonId)
                .ToListAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<Relation> relations)
        {
            _dbContext.Relations.RemoveRange(relations);
            await _dbContext.SaveChangesAsync();
        }
    }
}