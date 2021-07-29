using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NaturalPersonsDirectory.Common;
using NaturalPersonsDirectory.Db;
using NaturalPersonsDirectory.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalPersonsDirectory.Modules
{
    public class NaturalPersonService : INaturalPersonService
    {
        private readonly ApplicationDbContext _context;
        public NaturalPersonService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response<NaturalPersonResponse>> Create(NaturalPersonRequest request)
        {
            var naturalPersonWithSamePassportNumber = 
                await _context
                    .NaturalPersons
                    .FirstOrDefaultAsync(person => person.PassportNumber == request.PassportNumber);

            if (NaturalPersonExists(naturalPersonWithSamePassportNumber))
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.PassportNumberExists);
            }

            var naturalPerson = new NaturalPerson()
            {
                FirstNameEn = request.FirstNameEn,
                FirstNameGe = request.FirstNameGe,
                LastNameEn = request.LastNameEn,
                LastNameGe = request.LastNameGe,
                Address = request.Address,
                PassportNumber = request.PassportNumber,
                Birthday = DateTime.Parse(request.Birthday),
                ContactInformation = request.ContactInformation,
            };

            _context.NaturalPersons.Add(naturalPerson);
            await _context.SaveChangesAsync();

            var response = new NaturalPersonResponse(naturalPerson);

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.Create, response);
        }

        public async Task<Response<NaturalPersonResponse>> Delete(int id)
        {
            var naturalPerson =
                await _context
                    .NaturalPersons
                    .SingleOrDefaultAsync(person => person.Id == id);

            if (!NaturalPersonExists(naturalPerson))
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.IdNotExists);
            }

            var relations =
                await _context
                    .Relations
                    .Where(relation => relation.FromId == naturalPerson.Id || relation.ToId == naturalPerson.Id)
                    .ToListAsync();
            
            _context.RemoveRange(relations);

            _context.NaturalPersons.Remove(naturalPerson);
            await _context.SaveChangesAsync();

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.Delete, new NaturalPersonResponse());
        }

        public async Task<Response<NaturalPersonResponse>> GetAll(PaginationParameters parameters)
        {
            var orderProperty = typeof(NaturalPerson).GetProperty(parameters.OrderBy);
            var naturalPersons = await _context
                .NaturalPersons
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            var naturalPersonsExist = naturalPersons.Any();
            if (!naturalPersonsExist)
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.NotFound);
            }

            var listShouldBeOrdered = orderProperty != null;
            var validationPropertyIsCorrect = Validator.IsValidOrder(parameters.OrderBy);
            if (listShouldBeOrdered && validationPropertyIsCorrect)
            {
                naturalPersons = naturalPersons.OrderBy(property => orderProperty.GetValue(property, null)).ToList();
            }

            var response = new NaturalPersonResponse(naturalPersons);

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.Success, response);
        }

        public async Task<Response<NaturalPersonResponse>> GetById(int id)
        {
            var naturalPerson = 
                await _context
                    .NaturalPersons
                    .FirstOrDefaultAsync(person => person.Id == id);

            if (!NaturalPersonExists(naturalPerson))
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.NotFound);
            }

            var response = new NaturalPersonResponse(naturalPerson);

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.Success, response);
        }

        public async Task<Response<NaturalPersonResponse>> Update(int id, NaturalPersonRequest request)
        {
            var naturalPerson = await _context.NaturalPersons.SingleOrDefaultAsync(person => person.Id == id);

            if (!NaturalPersonExists(naturalPerson))
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.IdNotExists);
            }

            var passportNumberHasChanged = naturalPerson.PassportNumber != request.PassportNumber;
            if (passportNumberHasChanged)
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.ChangingPassportNumberNotAllowed);
            }

            naturalPerson.FirstNameEn = request.FirstNameEn;
            naturalPerson.FirstNameGe = request.FirstNameGe;
            naturalPerson.LastNameEn = request.LastNameEn;
            naturalPerson.LastNameGe = request.LastNameGe;
            naturalPerson.Address = request.Address;
            naturalPerson.Birthday = DateTime.Parse(request.Birthday);
            naturalPerson.ContactInformation = request.ContactInformation;

            _context.NaturalPersons.Update(naturalPerson);
            await _context.SaveChangesAsync();

            var response = new NaturalPersonResponse(naturalPerson);

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.Update, response);
        }

        public async Task<Response<RelatedPersonsResponse>> GetRelatedPersons(int id)
        {
            var naturalPerson = 
                await _context
                    .NaturalPersons
                    .SingleOrDefaultAsync(person => person.Id == id);

            if (!NaturalPersonExists(naturalPerson))
            {
                return ResponseHelper<RelatedPersonsResponse>.GetResponse(StatusCode.IdNotExists);
            }

            var relationsTo = await 
                _context
                    .Relations
                    .Where(relation => relation.FromId == id)
                    .Include(relation => relation.To)
                    .Select(relation => GetRelatedPerson(relation.To, relation.RelationType))
                    .ToListAsync();

            var relationsFrom = await 
                _context
                    .Relations
                    .Where(relation => relation.ToId == id)
                    .Include(relation => relation.From)
                    .Select(relation => GetRelatedPerson(relation.From, relation.RelationType))
                    .ToListAsync();

            var relatedPersons = new List<RelatedPerson>();
            relatedPersons.AddRange(relationsTo);
            relatedPersons.AddRange(relationsFrom);

            var relatedPersonsExist = relatedPersons.Any();
            if (!relatedPersonsExist)
            {
                return ResponseHelper<RelatedPersonsResponse>.GetResponse(StatusCode.NotFound);
            }

            var response = new RelatedPersonsResponse(relatedPersons);

            return ResponseHelper<RelatedPersonsResponse>.GetResponse(StatusCode.Success, response);
        }

        public async Task<Response<NaturalPersonResponse>> AddImage(int id, IFormFile file)
        {
            return await GetAddOrUpdateImageResponse(id, file, StatusCode.ImageAdded);
        }

        public async Task<Response<NaturalPersonResponse>> UpdateImage(int id, IFormFile file)
        {
            return await GetAddOrUpdateImageResponse(id, file, StatusCode.ImageUpdated);
        }

        public async Task<Response<NaturalPersonResponse>> DeleteImage(int id)
        {
            var naturalPerson = await _context.NaturalPersons.SingleOrDefaultAsync(person => person.Id == id);

            if (!NaturalPersonExists(naturalPerson))
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.IdNotExists);
            }

            var naturalPersonDoesNotHaveImage = string.IsNullOrWhiteSpace(naturalPerson.ImagePath);
            if (naturalPersonDoesNotHaveImage)
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.NoImage);
            }

            naturalPerson.ImagePath = null;

            _context.Update(naturalPerson);
            await _context.SaveChangesAsync();

            var response = new NaturalPersonResponse(naturalPerson);

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.ImageDeleted, response);
        }

        private async Task<Response<NaturalPersonResponse>> GetAddOrUpdateImageResponse(int id, IFormFile file, StatusCode statusCodeToReturnIfSuccess)
        {
            if (!Validator.IsValidImage(file))
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.UnsupportedFileFormat);
            }

            var naturalPerson = await _context.NaturalPersons.SingleOrDefaultAsync(person => person.Id == id);

            if (!NaturalPersonExists(naturalPerson))
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.IdNotExists);
            }

            var naturalPersonDoesNotHaveImage = string.IsNullOrWhiteSpace(naturalPerson.ImagePath);
            if (naturalPersonDoesNotHaveImage)
            {
                if(statusCodeToReturnIfSuccess == StatusCode.ImageUpdated)
                    return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.NoImage);
            }
            else
            {
                if(statusCodeToReturnIfSuccess == StatusCode.ImageAdded)
                    return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.AlreadyHaveImage);
            }
            
            naturalPerson.ImagePath = UploadImageAndGetPath(file);

            _context.Update(naturalPerson);
            await _context.SaveChangesAsync();

            var response = new NaturalPersonResponse(naturalPerson);

            return ResponseHelper<NaturalPersonResponse>.GetResponse(statusCodeToReturnIfSuccess, response);
        }
        
        private static string UploadImageAndGetPath(IFormFile image)
        {
            const string folderName = "Images\\";
            var folderPath = Path.Combine(Environment.CurrentDirectory, folderName);
            var fileExtension = image.FileName.Split('.').Last();
            var fileName = Guid.NewGuid().ToString() + '.' + fileExtension;
            var filePath = folderPath + fileName;

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(fileStream);
                fileStream.Flush();
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

        private static bool NaturalPersonExists(NaturalPerson naturalPerson)
        {
            return naturalPerson != null;
        }
    }
}
