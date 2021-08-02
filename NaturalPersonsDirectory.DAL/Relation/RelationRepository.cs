using Microsoft.EntityFrameworkCore;
using NaturalPersonsDirectory.Db;
using NaturalPersonsDirectory.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<ICollection<Relation>> GetAllAsync()
        {
            return await _dbContext
                .Relations
                .Include(relation => relation.From)
                .Include(relation => relation.To)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ICollection<Relation>> GetAllWithPaginationAsync(int skip, int take, bool orderByDescending)
        {
            return await _dbContext
                .Relations
                .OrderBy(orderByDescending)
                .Include(relation => relation.From)
                .Include(relation => relation.To)
                .Skip(skip)
                .AsNoTracking()
                .Take(take)
                .ToListAsync();
        }

        public async Task<Relation> GetByIdAsync(int id)
        {
            return await _dbContext
                .Relations
                .Include(relation => relation.From)
                .Include(relation => relation.To)
                .FirstOrDefaultAsync(relation => relation.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext
                .Relations
                .AnyAsync(relation => relation.Id == id);
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

        public async Task<bool> RelationWithGivenIdsExistAsync(int fromId, int toId)
        {
            return await _dbContext
                .Relations
                .AnyAsync(relation =>
                    relation.FromId == fromId && relation.ToId == toId
                    || relation.ToId == fromId && relation.FromId == toId);
        }

        public async Task<ICollection<Relation>> GetNaturalPersonRelationsAsync(int naturalPersonId)
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