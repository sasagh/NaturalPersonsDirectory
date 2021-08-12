using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NaturalPersonsDirectory.Db;
using NaturalPersonsDirectory.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalPersonsDirectory.DAL
{
    public class NaturalPersonRepository : INaturalPersonRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public NaturalPersonRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<NaturalPerson> CreateAsync(NaturalPerson naturalPerson)
        {
            _dbContext.NaturalPersons.Add(naturalPerson);
            await _dbContext.SaveChangesAsync();

            return naturalPerson;
        }

        public async Task<ICollection<NaturalPerson>> GetAllAsync()
        {
            return await _dbContext.NaturalPersons.AsNoTracking().ToListAsync();
        }

        public async Task<ICollection<NaturalPerson>> GetAllWithPaginationAsync(
            int skip,
            int take,
            bool orderByDescending,
            string orderBy = "")
        {
            return await _dbContext
                .NaturalPersons
                .OrderBy(orderByDescending, orderBy)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<NaturalPerson> GetByIdAsync(int id)
        {
            return await _dbContext.NaturalPersons.FirstOrDefaultAsync(naturalPerson => naturalPerson.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbContext
                .NaturalPersons
                .AnyAsync(naturalPerson => naturalPerson.Id == id);
        }

        public async Task<NaturalPerson> UpdateAsync(NaturalPerson naturalPerson)
        {
            _dbContext.NaturalPersons.Update(naturalPerson);
            await _dbContext.SaveChangesAsync();

            return naturalPerson;
        }

        public async Task DeleteAsync(NaturalPerson naturalPerson)
        {
            _dbContext.NaturalPersons.Remove(naturalPerson);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<NaturalPerson> GetByPassportNumberAsync(string passportNumber)
        {
            return await _dbContext.NaturalPersons.FirstOrDefaultAsync(naturalPerson =>
                    naturalPerson.PassportNumber == passportNumber);
        }

        public async Task<ICollection<RelatedPerson>> GetRelatedPersonsAsync(int naturalPersonId)
        {
            var relationsTo =
                await _dbContext
                    .Relations
                    .Where(relation => relation.FromId == naturalPersonId)
                    .Include(relation => relation.To)
                    .Select(relation => GetRelatedPerson(relation.To, relation.RelationType)).ToListAsync();

            var relationsFrom =
                await _dbContext
                    .Relations
                    .Where(relation => relation.ToId == naturalPersonId)
                    .Include(relation => relation.From)
                    .Select(relation => GetRelatedPerson(relation.From, relation.RelationType)).ToListAsync();

            var relatedPersons = new List<RelatedPerson>();
            relatedPersons.AddRange(relationsTo);
            relatedPersons.AddRange(relationsFrom);

            return relatedPersons;
        }

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            var folderPath = Path.Combine(Environment.CurrentDirectory, "Images\\");
            var fileExtension = image.FileName.Split('.').Last();
            var fileName = Guid.NewGuid().ToString() + '.' + fileExtension;

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = folderPath + fileName;

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }

            return fileName;
        }

        private static RelatedPerson GetRelatedPerson(NaturalPerson naturalPerson, string relationType)
        {
            var serializedNaturalPerson = JsonConvert.SerializeObject(naturalPerson);
            var relatedPerson = JsonConvert.DeserializeObject<RelatedPerson>(serializedNaturalPerson);
            relatedPerson.RelationType = relationType;

            return relatedPerson;
        }
    }
}