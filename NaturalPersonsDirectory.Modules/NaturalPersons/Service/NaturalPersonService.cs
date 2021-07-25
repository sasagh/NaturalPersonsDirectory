using Microsoft.AspNetCore.Http;
using NaturalPersonsDirectory.Common;
using NaturalPersonsDirectory.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NaturalPersonsDirectory.DAL;

namespace NaturalPersonsDirectory.Modules
{
    public class NaturalPersonService : INaturalPersonService
    {
        private readonly INaturalPersonRepository _npRepository;
        private readonly IRelationRepository _relationRepository;
        private readonly Logger _logger;
        public NaturalPersonService(INaturalPersonRepository npRepository, IRelationRepository relationRepository)
        {
            _npRepository = npRepository;
            _relationRepository = relationRepository;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<Response<NaturalPersonResponse>> Create(NaturalPersonRequest request)
        {
            var naturalPersonWithSamePassportNumber =
                _npRepository.GetByPassportNumberAsync(request.PassportNumber);

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

            await _npRepository.CreateAsync(naturalPerson);

            var response = new NaturalPersonResponse()
            {
                NaturalPersons = new List<NaturalPerson>() { naturalPerson }
            };

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.Success, response);
        }

        public async Task<Response<NaturalPersonResponse>> Delete(int id)
        {
            var naturalPerson = await _npRepository.GetByIdAsync(id);

            if (naturalPerson == null)
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.IdNotExists);
            }

            var relations = await _relationRepository.GetNaturalPersonRelationsAsync(naturalPerson.Id);

            await _relationRepository.DeleteRangeAsync(relations);
            await _npRepository.DeleteAsync(naturalPerson);

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.Delete, new NaturalPersonResponse());
        }

        public async Task<Response<NaturalPersonResponse>> GetAll(PaginationParameters parameters)
        {
            var prop = typeof(NaturalPerson).GetProperty(parameters.OrderBy);
            var naturalPersons = 
                await _npRepository.GetAllWithParametersAsync((parameters.PageNumber - 1) * parameters.PageSize, parameters.PageSize);

            var naturalPersonsList = naturalPersons.ToList();

            if (naturalPersonsList.Count == 0)
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.NotFound);
            }
            
            if (Validator.IsValidOrder(parameters.OrderBy) && prop != null)
            {
                naturalPersonsList = naturalPersonsList.OrderBy(property => prop.GetValue(property, null)).ToList();
            }

            var response = new NaturalPersonResponse()
            {
                NaturalPersons = naturalPersonsList
            };

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.Success, response);
        }

        public async Task<Response<NaturalPersonResponse>> GetById(int id)
        {
            var naturalPerson = await _npRepository.GetByIdAsync(id);

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
            var naturalPerson = await _npRepository.GetByIdAsync(id);

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

            await _npRepository.UpdateAsync(naturalPerson);

            var response = new NaturalPersonResponse()
            {
                NaturalPersons = new List<NaturalPerson>() { naturalPerson }
            };

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.Update, response);
        }

        public async Task<Response<RelatedPersonsResponse>> GetRelatedPersons(int id)
        {
            var naturalPerson = await _npRepository.GetByIdAsync(id);

            if (naturalPerson == null)
            {
                return ResponseHelper<RelatedPersonsResponse>.GetResponse(StatusCode.IdNotExists);
            }

            var relatedPersons = await _npRepository.GetRelatedPersonsAsync(id);
            
            var response = new RelatedPersonsResponse()
            {
                RelatedPersons = relatedPersons
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
            var naturalPerson = await _npRepository.GetByIdAsync(id);

            if (naturalPerson == null)
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.IdNotExists);
            }

            if (string.IsNullOrWhiteSpace(naturalPerson.ImagePath))
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.NoImage);
            }

            naturalPerson.ImagePath = null;

            await _npRepository.UpdateAsync(naturalPerson);

            var response = new NaturalPersonResponse()
            {
                NaturalPersons = new List<NaturalPerson>() { naturalPerson }
            };

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.ImageDeleted, response);
        }

        private async Task<Response<NaturalPersonResponse>> GetAddOrUpdateImageResponse(int id, IFormFile file, StatusCode statusCodeToReturnIfSuccess)
        {
            if (!Validator.IsValidImage(file))
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.UnsupportedFileFormat);
            }

            var naturalPerson = await _npRepository.GetByIdAsync(id);

            if (naturalPerson == null)
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.IdNotExists);
            }

            if (string.IsNullOrWhiteSpace(naturalPerson.ImagePath))
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.NoImage);
            }

            naturalPerson.ImagePath = await UploadImageAndGetPath(file);

            await _npRepository.UpdateAsync(naturalPerson);

            var response = new NaturalPersonResponse()
            {
                NaturalPersons = new List<NaturalPerson>() { naturalPerson }
            };

            return ResponseHelper<NaturalPersonResponse>.GetResponse(statusCodeToReturnIfSuccess, response);
        }
        
        private async Task<string> UploadImageAndGetPath(IFormFile image)
        {
            return await _npRepository.UploadImageAsync(image);
        }
    }
}
