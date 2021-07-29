using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NaturalPersonsDirectory.Common;
using NaturalPersonsDirectory.Db;
using NaturalPersonsDirectory.Models;
using Newtonsoft.Json;

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

        public async Task<IEnumerable<NaturalPerson>> GetAllAsync()
        {
            return await _dbContext.NaturalPersons.ToListAsync();
        }

        public async Task<IEnumerable<NaturalPerson>> GetAllWithParametersAsync(int skip, int take)
        {
            return await _dbContext
                .NaturalPersons
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<NaturalPerson> GetByIdAsync(int id)
        {
            return 
                await _dbContext.NaturalPersons.FirstOrDefaultAsync(naturalPerson => naturalPerson.Id == id);
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

        public async  Task<NaturalPerson> GetByPassportNumberAsync(string passportNumber)
        {
            return await _dbContext.NaturalPersons.FirstOrDefaultAsync(naturalPerson =>
                    naturalPerson.PassportNumber == passportNumber);
        }

        public async Task<IEnumerable<RelatedPerson>> GetRelatedPersonsAsync(int naturalPersonId)
        {
            var relationsTo =
                _dbContext
                    .Relations
                    .Where(relation => relation.FromId == naturalPersonId)
                    .Include(relation => relation.To)
                    .Select(relation => GetRelatedPerson(relation.From, relation.RelationType));
            
            var relationsFrom =
                _dbContext
                    .Relations
                    .Where(relation => relation.ToId == naturalPersonId)
                    .Include(relation => relation.From)
                    .Select(relation => GetRelatedPerson(relation.From, relation.RelationType));

            return await relationsTo.Concat(relationsFrom).ToListAsync();
        }

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            var folderPath= Path.Combine(Environment.CurrentDirectory, "Images\\");
            var fileName = new Guid().ToString();
            
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

            return filePath;
        }

        private static RelatedPerson GetRelatedPerson(NaturalPerson naturalPerson, RelationType relationType)
        {
            var serializedNaturalPerson = JsonConvert.SerializeObject(naturalPerson);
            var relatedPerson = JsonConvert.DeserializeObject<RelatedPerson>(serializedNaturalPerson);
            relatedPerson.RelationType = relationType;

            return relatedPerson;
        }
    }
}