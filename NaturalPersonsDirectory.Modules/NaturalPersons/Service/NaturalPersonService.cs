using Microsoft.AspNetCore.Http;
using NaturalPersonsDirectory.Common;
using NaturalPersonsDirectory.DAL;
using NaturalPersonsDirectory.Models;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
                await _npRepository.GetByPassportNumberAsync(request.PassportNumber);

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

            await _npRepository.CreateAsync(naturalPerson);

            var response = new NaturalPersonResponse(naturalPerson);

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.Create, response);
        }

        public async Task<Response<NaturalPersonResponse>> Delete(int id)
        {
            var naturalPerson = await _npRepository.GetByIdAsync(id);

            if (!NaturalPersonExists(naturalPerson))
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
            var naturalPersons =
                await _npRepository.GetAllWithPagination((parameters.PageNumber - 1) * parameters.PageSize, parameters.PageSize);

            var naturalPersonsList = naturalPersons.ToList();

            if (naturalPersonsList.Count == 0)
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.NotFound);
            }

            var response = new NaturalPersonResponse(naturalPersonsList);

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.Success, response);
        }

        public async Task<Response<NaturalPersonResponse>> GetById(int id)
        {
            var naturalPerson = await _npRepository.GetByIdAsync(id);

            if (!NaturalPersonExists(naturalPerson))
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.NotFound);
            }

            var response = new NaturalPersonResponse(naturalPerson);

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.Success, response);
        }

        public async Task<Response<NaturalPersonResponse>> Update(int id, NaturalPersonRequest request)
        {
            var naturalPerson = await _npRepository.GetByIdAsync(id);

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

            await _npRepository.UpdateAsync(naturalPerson);

            var response = new NaturalPersonResponse(naturalPerson);

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.Update, response);
        }

        public async Task<Response<RelatedPersonsResponse>> GetRelatedPersons(int id)
        {
            var naturalPerson = await _npRepository.GetByIdAsync(id);

            if (!NaturalPersonExists(naturalPerson))
            {
                return ResponseHelper<RelatedPersonsResponse>.GetResponse(StatusCode.IdNotExists);
            }

            var related = await _npRepository.GetRelatedPersonsAsync(id);
            var relatedPersons = related.ToList();

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
            var naturalPerson = await _npRepository.GetByIdAsync(id);

            if (!NaturalPersonExists(naturalPerson))
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.IdNotExists);
            }

            var naturalPersonDoesNotHaveImage = string.IsNullOrWhiteSpace(naturalPerson.ImageFileName);
            if (naturalPersonDoesNotHaveImage)
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.NoImage);
            }

            naturalPerson.ImageFileName = null;

            await _npRepository.UpdateAsync(naturalPerson);

            var response = new NaturalPersonResponse(naturalPerson);

            return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.ImageDeleted, response);
        }

        private async Task<Response<NaturalPersonResponse>> GetAddOrUpdateImageResponse(int id, IFormFile file, StatusCode statusCodeToReturnIfSuccess)
        {
            if (!Validator.IsValidImage(file))
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.UnsupportedFileFormat);
            }

            var naturalPerson = await _npRepository.GetByIdAsync(id);

            if (!NaturalPersonExists(naturalPerson))
            {
                return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.IdNotExists);
            }

            var naturalPersonDoesNotHaveImage = string.IsNullOrWhiteSpace(naturalPerson.ImageFileName);
            if (naturalPersonDoesNotHaveImage)
            {
                if (statusCodeToReturnIfSuccess == StatusCode.ImageUpdated)
                    return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.NoImage);
            }

            else
            {
                if (statusCodeToReturnIfSuccess == StatusCode.ImageAdded)
                    return ResponseHelper<NaturalPersonResponse>.GetResponse(StatusCode.AlreadyHaveImage);
            }

            naturalPerson.ImageFileName = await UploadImageAndGetFileName(file);

            await _npRepository.UpdateAsync(naturalPerson);

            var response = new NaturalPersonResponse(naturalPerson);

            return ResponseHelper<NaturalPersonResponse>.GetResponse(statusCodeToReturnIfSuccess, response);
        }

        private async Task<string> UploadImageAndGetFileName(IFormFile image)
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
                await image.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }

            return await _npRepository.UploadImageAsync(image);
        }

        private static bool NaturalPersonExists(NaturalPerson naturalPerson)
        {
            return naturalPerson != null;
        }
    }
}
