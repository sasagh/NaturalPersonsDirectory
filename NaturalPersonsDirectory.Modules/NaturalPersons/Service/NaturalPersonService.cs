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
        private readonly Logger _logger;
        public NaturalPersonService(ApplicationDbContext context)
        {
            _context = context;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<Response<NaturalPersonResponse>> Create(NaturalPersonRequest request)
        {
            var naturalPersonWithSamePassportNumber = await _context.NaturalPersons.FirstOrDefaultAsync(naturalPerson => naturalPerson.PassportNumber == request.PassportNumber);

            if (naturalPersonWithSamePassportNumber != null)
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.PassportNumberExists);
            }

            var naturalPerson = new NaturalPerson()
            {
                FirstNameEn = request.FirstNameEn,
                FirstNameGe = request.FirstNameGE,
                LastNameEn = request.LastNameEn,
                LastNameGe = request.LastNameGe,
                Address = request.Address,
                PassportNumber = request.PassportNumber,
                Birthday = DateTime.Parse(request.Birthday),
                ContactInformation = request.ContactInformation,
            };

            _context.NaturalPersons.Add(naturalPerson);
            await _context.SaveChangesAsync();

            var response = new NaturalPersonResponse()
            {
                NaturalPersons = new List<NaturalPerson>() { naturalPerson }
            };

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.Success, response);
        }

        public async Task<Response<NaturalPersonResponse>> Delete(int id)
        {
            var naturalPerson = await _context.NaturalPersons.SingleOrDefaultAsync(naturalPerson => naturalPerson.Id == id);

            if (naturalPerson == null)
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.IdNotExists);
            }

            var relations = await _context.Relations.Where(relation => relation.FromId == naturalPerson.Id || relation.ToId == naturalPerson.Id).ToListAsync();

            if (relations.Any())
            {
                foreach (var relation in relations)
                {
                    _context.Relations.Remove(relation);
                }
            }

            _context.NaturalPersons.Remove(naturalPerson);
            await _context.SaveChangesAsync();

            var response = new NaturalPersonResponse()
            {
                NaturalPersons = new List<NaturalPerson>()
            };

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.Delete, response);
        }

        public async Task<Response<NaturalPersonResponse>> GetAll(PaginationParameters parameters)
        {
            var prop = typeof(NaturalPerson).GetProperty(parameters.OrderBy);
            var naturalPersons = await _context
                .NaturalPersons
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            if (!naturalPersons.Any())
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.NotFound);
            }
            
            if (Validator.IsValidOrder(parameters.OrderBy) && prop != null)
            {
                naturalPersons = naturalPersons.OrderBy(property => prop.GetValue(property, null)).ToList();
            }

            var response = new NaturalPersonResponse()
            {
                NaturalPersons = naturalPersons.Any() ? naturalPersons : new List<NaturalPerson>()
            };

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.Success, response);
        }

        public async Task<Response<NaturalPersonResponse>> GetById(int id)
        {
            var naturalPerson = await _context.NaturalPersons.FirstOrDefaultAsync(naturalPerson => naturalPerson.Id == id);

            if (naturalPerson == null)
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.NotFound);
            }
            
            var response = new NaturalPersonResponse()
            {
                NaturalPersons = new List<NaturalPerson>() { naturalPerson }
            };

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.Success, response);
        }

        public async Task<Response<NaturalPersonResponse>> Update(int id, NaturalPersonRequest request)
        {
            var naturalPerson = await _context.NaturalPersons.SingleOrDefaultAsync(person => person.Id == id);

            if (naturalPerson == null)
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.IdNotExists);
            }

            if (naturalPerson.PassportNumber != request.PassportNumber)
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.ChangingPassportNumberNotAllowed);
            }

            naturalPerson.FirstNameEn = request.FirstNameEn;
            naturalPerson.FirstNameGe = request.FirstNameGE;
            naturalPerson.LastNameEn = request.LastNameEn;
            naturalPerson.LastNameGe = request.LastNameGe;
            naturalPerson.Address = request.Address;
            naturalPerson.Birthday = DateTime.Parse(request.Birthday);
            naturalPerson.ContactInformation = request.ContactInformation;

            _context.NaturalPersons.Update(naturalPerson);
            await _context.SaveChangesAsync();

            var response = new NaturalPersonResponse()
            {
                NaturalPersons = new List<NaturalPerson>() { naturalPerson }
            };

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.Update, response);
        }

        public async Task<Response<RelatedPersonsResponse>> GetRelatedPersons(int id)
        {
            var naturalPerson = await _context.NaturalPersons.SingleOrDefaultAsync(naturalPerson => naturalPerson.Id == id);

            if (naturalPerson == null)
            {
                return ResponseHelper<RelatedPersonsResponse>.GetResponse(StatusCode.IdNotExists);
            }

            var relationsFrom = await _context.Relations.Where(relation => relation.FromId == id).Include(relation => relation.To).ToListAsync();
            var relationsTo = await _context.Relations.Where(relation => relation.ToId == id).Include(relation => relation.From).ToListAsync();

            var relatedPersons = new List<RelatedPerson>();

            if (relationsFrom.Any())
            {
                foreach (var relation in relationsFrom)
                {
                    var serializedNaturalPerson = JsonConvert.SerializeObject(relation.To);
                    var relatedPerson = JsonConvert.DeserializeObject<RelatedPerson>(serializedNaturalPerson);
                    relatedPerson.RelationType = relation.RelationType;

                    relatedPersons.Add(relatedPerson);
                }
            }

            if (relationsTo.Any())
            {
                foreach (var relation in relationsTo)
                {
                    var serializedNaturalPerson = JsonConvert.SerializeObject(relation.From);
                    var relatedPerson = JsonConvert.DeserializeObject<RelatedPerson>(serializedNaturalPerson);
                    relatedPerson.RelationType = relation.RelationType;

                    relatedPersons.Add(relatedPerson);
                }
            }
            
            if (!relatedPersons.Any())
            {
                return ResponseHelper<RelatedPersonsResponse>.GetResponse(StatusCode.NotFound);
            }
            
            var response = new RelatedPersonsResponse()
            {
                RelatedPersons = relatedPersons.Any() ? relatedPersons : new List<RelatedPerson>()
            };

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

            if (naturalPerson == null)
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.IdNotExists);
            }

            if (string.IsNullOrWhiteSpace(naturalPerson.ImagePath))
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.NoImage);
            }

            naturalPerson.ImagePath = null;

            _context.Update(naturalPerson);
            await _context.SaveChangesAsync();

            var response = new NaturalPersonResponse()
            {
                NaturalPersons = new List<NaturalPerson>() { naturalPerson }
            };

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.ImageDeleted, response);
        }

        private async Task<Response<NaturalPersonResponse>> GetAddOrUpdateImageResponse(int id, IFormFile file, StatusCode statusCodeToReturnIfSuccess)
        {
            if (file == null || !Validator.IsValidImage(file))
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.UnsupportedFileFormat);
            }

            var naturalPerson = await _context.NaturalPersons.SingleOrDefaultAsync(person => person.Id == id);

            if (naturalPerson == null)
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.IdNotExists);
            }

            if (string.IsNullOrWhiteSpace(naturalPerson.ImagePath))
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.NoImage);
            }

            naturalPerson.ImagePath = UploadImageAndGetPath(naturalPerson, file);

            _context.Update(naturalPerson);
            await _context.SaveChangesAsync();

            var response = new NaturalPersonResponse()
            {
                NaturalPersons = new List<NaturalPerson>() { naturalPerson }
            };

            return ResponseHelper<NaturalPersonResponse>.GetResponse(statusCodeToReturnIfSuccess, response);
        }
        
        private static string UploadImageAndGetPath(NaturalPerson naturalPerson, IFormFile image)
        {
            var folderName = Path.Combine(Environment.CurrentDirectory, "Images\\");

            var fileName = new Guid().ToString();

            var filePath = folderName + fileName;

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(fileStream);
                fileStream.Flush();
            }

            return filePath;
        }
    }
}
