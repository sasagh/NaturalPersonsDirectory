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
        private readonly NaturalPersonsDirectoryDbContext _context;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public NaturalPersonService(NaturalPersonsDirectoryDbContext context)
        {
            _context = context;
        }

        public async Task<IResponse<NaturalPersonResponse>> Create(NaturalPersonRequest request)
        {
            try
            {
                var naturalPersonWithSamePassportNumber = await _context.NaturalPersons.FirstOrDefaultAsync(naturalPerson => naturalPerson.PassportNumber == request.PassportNumber);

                if (naturalPersonWithSamePassportNumber != null)
                {
                    return ResponseHelper.Fail<NaturalPersonResponse>(StatusCode.PassportNumberExists);
                }

                var naturalPerson = new NaturalPerson()
                {
                    FirstNameEn = request.FirstNameEn,
                    FirstNameGE = request.FirstNameGE,
                    LastNameEn = request.LastNameEn,
                    LastNameGe = request.LastNameGe,
                    Address = request.Address,
                    PassportNumber = request.PassportNumber,
                    Birthday = DateTime.Parse(request.Birthday),
                    ContactInformations = request.ContactInformations,
                };

                _context.NaturalPersons.Add(naturalPerson);
                await _context.SaveChangesAsync();

                var response = new NaturalPersonResponse()
                {
                    NaturalPersons = new List<NaturalPerson>() { naturalPerson }
                };

                return ResponseHelper.Ok(response);
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        public async Task<IResponse<NaturalPersonResponse>> Delete(int id)
        {
            try
            {
                var naturalPerson = await _context.NaturalPersons.SingleOrDefaultAsync(naturalPerson => naturalPerson.NaturalPersonId == id);

                if (naturalPerson == null)
                {
                    return ResponseHelper.Fail<NaturalPersonResponse>(StatusCode.IdNotExists);
                }

                var relations = await _context.Relations.Where(relation => relation.FromId == naturalPerson.NaturalPersonId || relation.ToId == naturalPerson.NaturalPersonId).ToListAsync();

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

                return ResponseHelper.Ok(response, StatusCode.Delete);
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        public async Task<IResponse<NaturalPersonResponse>> Get(PaginationParameters parameters)
        {
            try
            {
                var prop = typeof(NaturalPerson).GetProperty(parameters.OrderBy);
                var naturalPersons = await _context
                    .NaturalPersons
                    .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                    .Take(parameters.PageSize)
                    .ToListAsync();

                if (ValidateFormat.ValidOrder(parameters.OrderBy) && prop != null)
                {
                    naturalPersons = naturalPersons.OrderBy(property => prop.GetValue(property, null)).ToList();
                }

                var response = new NaturalPersonResponse()
                {
                    NaturalPersons = naturalPersons.Any() ? naturalPersons : new List<NaturalPerson>()
                };

                return naturalPersons.Any() ? ResponseHelper.Ok(response) : ResponseHelper.NotFound(response);
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        public async Task<IResponse<NaturalPersonResponse>> GetById(int id)
        {
            try
            {
                var naturalPerson = await _context.NaturalPersons.FirstOrDefaultAsync(naturalPerson => naturalPerson.NaturalPersonId == id);

                var response = new NaturalPersonResponse()
                {
                    NaturalPersons = naturalPerson != null ? new List<NaturalPerson>() { naturalPerson } : new List<NaturalPerson>()
                };

                return naturalPerson != null ? ResponseHelper.Ok(response) : ResponseHelper.NotFound(response);
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        public async Task<IResponse<NaturalPersonResponse>> Update(int id, NaturalPersonRequest request)
        {
            try
            {
                var naturalPerson = await _context.NaturalPersons.SingleOrDefaultAsync(person => person.NaturalPersonId == id);

                if (naturalPerson == null)
                {
                    return ResponseHelper.Fail<NaturalPersonResponse>(StatusCode.IdNotExists);
                }

                if (naturalPerson.PassportNumber != request.PassportNumber)
                {
                    return ResponseHelper.Fail<NaturalPersonResponse>(StatusCode.ChangingPassportNumberNotAllowed);
                }

                naturalPerson.FirstNameEn = request.FirstNameEn;
                naturalPerson.FirstNameGE = request.FirstNameGE;
                naturalPerson.LastNameEn = request.LastNameEn;
                naturalPerson.LastNameGe = request.LastNameGe;
                naturalPerson.Address = request.Address;
                naturalPerson.Birthday = DateTime.Parse(request.Birthday);
                naturalPerson.ContactInformations = request.ContactInformations;

                _context.NaturalPersons.Update(naturalPerson);
                await _context.SaveChangesAsync();

                var response = new NaturalPersonResponse()
                {
                    NaturalPersons = new List<NaturalPerson>() { naturalPerson }
                };

                return ResponseHelper.Ok(response, StatusCode.Update);
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        public async Task<IResponse<RelatedPersonsResponse>> GetRelatedPersons(int id)
        {
            var naturalPerson = await _context.NaturalPersons.SingleOrDefaultAsync(naturalPerson => naturalPerson.NaturalPersonId == id);

            if (naturalPerson == null)
            {
                return ResponseHelper.Fail<RelatedPersonsResponse>(StatusCode.IdNotExists);
            }

            var relationsFrom = await _context.Relations.Where(relation => relation.FromId == id).Include(relation => relation.RelationTo).ToListAsync();
            var relationsTo = await _context.Relations.Where(relation => relation.ToId == id).Include(relation => relation.RelationFrom).ToListAsync();

            var relatedPersons = new List<RelatedPerson>();

            if (relationsFrom.Any())
            {
                foreach (var relation in relationsFrom)
                {
                    var serializedNaturalPerson = JsonConvert.SerializeObject(relation.RelationTo);
                    RelatedPerson relatedPerson = JsonConvert.DeserializeObject<RelatedPerson>(serializedNaturalPerson);
                    relatedPerson.RelationType = relation.RelationType;

                    relatedPersons.Add(relatedPerson);
                }
            }

            if (relationsTo.Any())
            {
                foreach (var relation in relationsTo)
                {
                    var serializedNaturalPerson = JsonConvert.SerializeObject(relation.RelationFrom);
                    RelatedPerson relatedPerson = JsonConvert.DeserializeObject<RelatedPerson>(serializedNaturalPerson);
                    relatedPerson.RelationType = relation.RelationType;

                    relatedPersons.Add(relatedPerson);
                }
            }

            var response = new RelatedPersonsResponse()
            {
                RelatedPersons = relatedPersons.Any() ? relatedPersons : new List<RelatedPerson>()
            };

            return relatedPersons.Any() ? ResponseHelper.Ok(response) : ResponseHelper.NotFound(response);
        }

        public async Task<IResponse<NaturalPersonResponse>> AddImage(int id, IFormFile image)
        {
            try
            {
                if (image == null)
                {
                    return ResponseHelper.Fail<NaturalPersonResponse>();
                }

                var naturalPerson = await _context.NaturalPersons.SingleOrDefaultAsync(person => person.NaturalPersonId == id);

                if (naturalPerson == null)
                {
                    return ResponseHelper.Fail<NaturalPersonResponse>(StatusCode.IdNotExists);
                }

                if (naturalPerson.ImagePath != null)
                {
                    return ResponseHelper.Fail<NaturalPersonResponse>(StatusCode.AlreadyHaveImage);
                }

                if (!ValidateFormat.ValidImage(image))
                {
                    return ResponseHelper.Fail<NaturalPersonResponse>(StatusCode.UnsupportedFileFormat);
                }

                UploadImage(naturalPerson, image);

                _context.Update(naturalPerson);
                await _context.SaveChangesAsync();

                var response = new NaturalPersonResponse()
                {
                    NaturalPersons = new List<NaturalPerson>() { naturalPerson }
                };

                return ResponseHelper.Ok(response, StatusCode.ImageAdded);

            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        public async Task<IResponse<NaturalPersonResponse>> UpdateImage(int id, IFormFile image)
        {
            try
            {
                if (image != null)
                {
                    return ResponseHelper.Fail<NaturalPersonResponse>();
                }

                var naturalPerson = await _context.NaturalPersons.SingleOrDefaultAsync(person => person.NaturalPersonId == id);

                if (naturalPerson == null)
                {
                    return ResponseHelper.Fail<NaturalPersonResponse>(StatusCode.IdNotExists);
                }

                if (string.IsNullOrWhiteSpace(naturalPerson.ImagePath))
                {
                    return ResponseHelper.Fail<NaturalPersonResponse>(StatusCode.NoImage);
                }

                if (!ValidateFormat.ValidImage(image))
                {
                    return ResponseHelper.Fail<NaturalPersonResponse>(StatusCode.UnsupportedFileFormat);
                }

                UploadImage(naturalPerson, image);

                _context.Update(naturalPerson);
                await _context.SaveChangesAsync();

                var response = new NaturalPersonResponse()
                {
                    NaturalPersons = new List<NaturalPerson>() { naturalPerson }
                };

                return ResponseHelper.Ok(response, StatusCode.ImageUpdated);
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        public async Task<IResponse<NaturalPersonResponse>> DeleteImage(int id)
        {
            try
            {
                var naturalPerson = await _context.NaturalPersons.SingleOrDefaultAsync(person => person.NaturalPersonId == id);

                if (naturalPerson == null)
                {
                    return ResponseHelper.Fail<NaturalPersonResponse>(StatusCode.IdNotExists);
                }

                if (string.IsNullOrWhiteSpace(naturalPerson.ImagePath))
                {
                    return ResponseHelper.Fail<NaturalPersonResponse>(StatusCode.NoImage);
                }

                naturalPerson.ImagePath = null;

                _context.Update(naturalPerson);
                await _context.SaveChangesAsync();

                var response = new NaturalPersonResponse()
                {
                    NaturalPersons = new List<NaturalPerson>() { naturalPerson }
                };

                return ResponseHelper.Ok(response, StatusCode.ImageDeleted);
            }
            catch (Exception ex)
            {
                return CatchException(ex);
            }
        }

        private void UploadImage(NaturalPerson naturalPerson, IFormFile image)
        {
            var folderName = Path.Combine(Environment.CurrentDirectory, "Images\\");

            var fileNameTemplate =
                naturalPerson.FirstNameEn.ToUpper() +
                '_' +
                naturalPerson.LastNameEn.ToUpper() +
                '_' +
                naturalPerson.PassportNumber;

            var fileName =
                fileNameTemplate +
                '_' +
                "(1)" +
                '.' +
                image.FileName.Split('.').Last();

            var filePath = folderName + fileName;

            if (File.Exists(filePath))
            {
                var sortedImages = new DirectoryInfo(folderName).GetFiles().OrderBy(file => file.LastWriteTime);
                var lastImageName = sortedImages.Reverse().ToList().FirstOrDefault(fileInfo => fileInfo.Name.Contains(fileNameTemplate)).Name;

                var fileNameParts = lastImageName.Split('.');
                var partWithNumber = fileNameParts[fileNameParts.Length - 2];
                var fileNameNumber = partWithNumber.Split('_').Last();
                var imageNumber = fileNameNumber.Remove(0, 1);
                imageNumber = imageNumber.Remove(imageNumber.Length - 1, 1);
                int.TryParse(imageNumber, out int number);
                var newNumber = '(' + (number + 1).ToString() + ')';
                fileName = fileName.Replace("(1)", newNumber);
                filePath = folderName + fileName;
            }

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(fileStream);
                fileStream.Flush();
            }

            naturalPerson.ImagePath = filePath;
        }

        private IResponse<NaturalPersonResponse> CatchException(Exception ex)
        {
            _logger.Error(ex.Message);
            return ResponseHelper.Fail<NaturalPersonResponse>();
        }
    }
}
