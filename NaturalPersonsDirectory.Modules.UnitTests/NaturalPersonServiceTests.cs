using Microsoft.AspNetCore.Http;
using Moq;
using NaturalPersonsDirectory.Common;
using NaturalPersonsDirectory.DAL;
using NaturalPersonsDirectory.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NaturalPersonsDirectory.Modules.UnitTests
{
    public class NaturalPersonServiceTests
    {
        private readonly NaturalPersonService _sut;
        private readonly Mock<INaturalPersonRepository> _npRepository = new Mock<INaturalPersonRepository>();
        private readonly Mock<IRelationRepository> _relationRepository = new Mock<IRelationRepository>();

        public NaturalPersonServiceTests()
        {
            _sut = new NaturalPersonService(_npRepository.Object, _relationRepository.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnFirstTenNaturalPersons_WhenPaginationParametersAreDefault()
        {
            //Arrange
            const int expectedResultDataSize = 10;
            const StatusCode expectedStatusCode = StatusCode.Success;
            var paginationParameters = new PaginationParameters();

            var naturalPersons = new Collection<NaturalPerson>();
            for (var i = 0; i < expectedResultDataSize; i++)
            {
                naturalPersons.Add(new NaturalPerson());
            }

            _npRepository.Setup(np =>
                np.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>()).Result).Returns(naturalPersons);

            //Act
            var methodResult = await _sut.GetAll(paginationParameters);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(expectedResultDataSize, methodResult.Data.Count);
        }

        [Fact]
        public async Task GetAll_ShouldReturnPageSizeCountElements_WhenPaginationParametersAreNotDefault()
        {
            //Arrange
            const int expectedResultDataSize = 2;
            const StatusCode expectedStatusCode = StatusCode.Success;
            var paginationParameters = new PaginationParameters()
            {
                PageSize = 2
            };

            var naturalPersons = new Collection<NaturalPerson>();
            for (var i = 0; i < expectedResultDataSize; i++)
            {
                naturalPersons.Add(new NaturalPerson());
            }

            _npRepository.Setup(np =>
                np.GetAllWithPagination(It.IsAny<int>(), expectedResultDataSize)).ReturnsAsync(naturalPersons);

            //Act
            var methodResult = await _sut.GetAll(paginationParameters);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(expectedResultDataSize, methodResult.Data.Count);
        }

        [Fact]
        public async Task GetAll_ShouldReturnNotFound_WhenThereIsNoItemInDatabase()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.NotFound;
            var paginationParameters = new PaginationParameters();

            var naturalPersons = new Collection<NaturalPerson>();

            _npRepository.Setup(np =>
                np.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(naturalPersons);

            //Act
            var methodResult = await _sut.GetAll(paginationParameters);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(0, methodResult.Data.Count);
        }

        [Fact]
        public async Task GetById_ShouldReturnNaturalPerson_WhenNaturalPersonWithGivenIdExists()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.Success;
            var naturalPerson = PreparedNaturalPersons.GetBidzinaTabagari();

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>()).Result).Returns(naturalPerson);

            //Act
            var methodResult = await _sut.GetById(It.IsAny<int>());

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(1, methodResult.Data.Count);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenNaturalPersonWithGivenIdDoesNotExist()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.NotFound;

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);

            //Act
            var methodResult = await _sut.GetById(It.IsAny<int>());

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(0, methodResult.Data.Count);
        }

        [Fact]
        public async Task Create_ShouldCreateNaturalPerson_WhenAllParametersAreValid()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.Create;
            var naturalPerson = PreparedNaturalPersons.GetBidzinaTabagari();
            var request = new NaturalPersonRequest()
            {
                Address = naturalPerson.Address,
                Birthday = naturalPerson.Birthday.ToString(CultureInfo.InvariantCulture),
                ContactInformation = naturalPerson.ContactInformation,
                PassportNumber = naturalPerson.PassportNumber,
                FirstNameEn = naturalPerson.FirstNameEn,
                FirstNameGe = naturalPerson.FirstNameGe,
                LastNameEn = naturalPerson.LastNameEn,
                LastNameGe = naturalPerson.LastNameGe
            };

            //Act
            var methodResult = await _sut.Create(request);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(naturalPerson.PassportNumber, methodResult.Data.NaturalPersons.First().PassportNumber);
        }

        [Fact]
        public async Task Create_ShouldNotCreateNaturalPerson_WhenPassportNumberAlreadyExists()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.PassportNumberExists;
            var naturalPerson = PreparedNaturalPersons.GetBidzinaTabagari();
            var request = new NaturalPersonRequest()
            {
                Address = naturalPerson.Address,
                Birthday = naturalPerson.Birthday.ToString(CultureInfo.InvariantCulture),
                ContactInformation = naturalPerson.ContactInformation,
                PassportNumber = naturalPerson.PassportNumber,
                FirstNameEn = naturalPerson.FirstNameEn,
                FirstNameGe = naturalPerson.FirstNameGe,
                LastNameEn = naturalPerson.LastNameEn,
                LastNameGe = naturalPerson.LastNameGe
            };

            _npRepository.Setup(np =>
                np.GetByPassportNumberAsync(It.IsAny<string>())).ReturnsAsync(naturalPerson);

            //Act
            var methodResult = await _sut.Create(request);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Null(methodResult.Data);
        }

        [Fact]
        public async Task Update_ShouldUpdateNaturalPerson_WhenAllParametersAreCorrect()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.Update;
            const string changedFirstName = "Leqso";
            var naturalPerson = PreparedNaturalPersons.GetBidzinaTabagari();
            var request = new NaturalPersonRequest()
            {
                Address = naturalPerson.Address,
                Birthday = naturalPerson.Birthday.ToString(CultureInfo.InvariantCulture),
                ContactInformation = naturalPerson.ContactInformation,
                PassportNumber = naturalPerson.PassportNumber,
                FirstNameEn = changedFirstName,
                FirstNameGe = naturalPerson.FirstNameGe,
                LastNameEn = naturalPerson.LastNameEn,
                LastNameGe = naturalPerson.LastNameGe
            };

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(naturalPerson);

            //Act
            var methodResult = await _sut.Update(It.IsAny<int>(), request);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(changedFirstName, methodResult.Data.NaturalPersons.First().FirstNameEn);
        }

        [Fact]
        public async Task Update_ShouldNotUpdateNaturalPerson_WhenNaturalPersonWithGivenIdDoesNotExist()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.IdNotExists;
            const string changedFirstName = "Leqso";
            var naturalPerson = PreparedNaturalPersons.GetBidzinaTabagari();
            var request = new NaturalPersonRequest()
            {
                Address = naturalPerson.Address,
                Birthday = naturalPerson.Birthday.ToString(CultureInfo.InvariantCulture),
                ContactInformation = naturalPerson.ContactInformation,
                PassportNumber = naturalPerson.PassportNumber,
                FirstNameEn = changedFirstName,
                FirstNameGe = naturalPerson.FirstNameGe,
                LastNameEn = naturalPerson.LastNameEn,
                LastNameGe = naturalPerson.LastNameGe
            };

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);

            //Act
            var methodResult = await _sut.Update(It.IsAny<int>(), request);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Null(methodResult.Data);
        }

        [Fact]
        public async Task Update_ShouldNotUpdateNaturalPerson_WhenPassportNumberHasChanged()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.ChangingPassportNumberNotAllowed;
            const string changedPassportNumber = "00000000000";
            var naturalPerson = PreparedNaturalPersons.GetBidzinaTabagari();
            var request = new NaturalPersonRequest()
            {
                Address = naturalPerson.Address,
                Birthday = naturalPerson.Birthday.ToString(CultureInfo.InvariantCulture),
                ContactInformation = naturalPerson.ContactInformation,
                PassportNumber = changedPassportNumber,
                FirstNameEn = naturalPerson.FirstNameEn,
                FirstNameGe = naturalPerson.FirstNameGe,
                LastNameEn = naturalPerson.LastNameEn,
                LastNameGe = naturalPerson.LastNameGe
            };

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(naturalPerson);

            //Act
            var methodResult = await _sut.Update(It.IsAny<int>(), request);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Null(methodResult.Data);
        }

        [Fact]
        public async Task Delete_ShouldDeleteNaturalPerson_WhenNaturalPersonWithGivenIdExist()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.Delete;
            var naturalPerson = PreparedNaturalPersons.GetBidzinaTabagari();
            var relations = new Collection<Relation>()
            {
                new Relation()
                {
                    Id = 1,
                    FromId = 1,
                    ToId = 2,
                    RelationType = Enum.GetName(typeof(RelationType), RelationType.Acquaintance)
                }
            };

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(naturalPerson);

            _relationRepository.Setup(relation =>
                relation.GetNaturalPersonRelationsAsync(It.IsAny<int>())).ReturnsAsync(relations);

            //Act
            var methodResult = await _sut.Delete(It.IsAny<int>());

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Null(methodResult.Data);
        }

        [Fact]
        public async Task Delete_ShouldReturnIdNotExists_WhenNaturalPersonWithGivenIdDoesNotExist()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.IdNotExists;

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);

            //Act
            var methodResult = await _sut.Delete(It.IsAny<int>());

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Null(methodResult.Data);
        }

        [Fact]
        public async Task GetRelatedPersons_ShouldReturnRelatedPersons_WhenNaturalPersonWithGivenIdHaveRelatedPersons()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.Success;
            var naturalPerson = PreparedNaturalPersons.GetBidzinaTabagari();
            var relatedPersons = new Collection<RelatedPerson>()
            {
                new RelatedPerson()
                {
                    Address = naturalPerson.Address,
                    Birthday = naturalPerson.Birthday,
                    ContactInformation = naturalPerson.ContactInformation,
                    PassportNumber = naturalPerson.PassportNumber,
                    FirstNameEn = naturalPerson.FirstNameEn,
                    FirstNameGe = naturalPerson.FirstNameGe,
                    LastNameEn = naturalPerson.LastNameEn,
                    LastNameGe = naturalPerson.LastNameGe,
                    RelationType = Enum.GetName(typeof(RelationType), RelationType.Acquaintance)
                }
            };

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(naturalPerson);

            _npRepository.Setup(np =>
                np.GetRelatedPersonsAsync(It.IsAny<int>())).ReturnsAsync(relatedPersons);

            //Act
            var methodResult = await _sut.GetRelatedPersons(It.IsAny<int>());

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(1, methodResult.Data.RelatedPersons.Count);
        }

        [Fact]
        public async Task GetRelatedPersons_ShouldReturnNotFound_WhenNaturalPersonWithGivenIdDoesNotHaveRelatedPersons()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.NotFound;
            var naturalPerson = PreparedNaturalPersons.GetBidzinaTabagari();

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(naturalPerson);

            _npRepository.Setup(np =>
                np.GetRelatedPersonsAsync(It.IsAny<int>())).ReturnsAsync(new Collection<RelatedPerson>());

            //Act
            var methodResult = await _sut.GetRelatedPersons(It.IsAny<int>());

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(0, methodResult.Data.Count);
        }

        [Fact]
        public async Task GetRelatedPersons_ShouldReturnIdNotExists_WhenNaturalPersonWithGivenIdDoesNotExist()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.IdNotExists;

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);

            //Act
            var methodResult = await _sut.GetRelatedPersons(It.IsAny<int>());

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Null(methodResult.Data);
        }

        [Fact]
        public async Task AddImage_ShouldAddImage_WhenAllParametersAreValid()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.ImageAdded;
            var naturalPerson = PreparedNaturalPersons.GetBidzinaTabagari();
            naturalPerson.ImageFileName = null;
            const string imageFileName = "image.jpg";
            var image = new FormFile(new MemoryStream(), 0, 0, "Data", imageFileName);

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(naturalPerson);

            _npRepository.Setup(np =>
                np.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync(imageFileName);

            //Act
            var methodResult = await _sut.AddImage(It.IsAny<int>(), image);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(imageFileName, methodResult.Data.NaturalPersons.First().ImageFileName);
        }

        [Fact]
        public async Task AddImage_ShouldNotAddImage_WhenFileFormatIsNotValid()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.UnsupportedFileFormat;
            var naturalPerson = PreparedNaturalPersons.GetBidzinaTabagari();
            naturalPerson.ImageFileName = null;
            const string imageFileName = "image.txt";
            var image = new FormFile(new MemoryStream(), 0, 0, "Data", imageFileName);

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(naturalPerson);

            _npRepository.Setup(np =>
                np.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync(imageFileName);

            //Act
            var methodResult = await _sut.AddImage(It.IsAny<int>(), image);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Null(methodResult.Data);
        }

        [Fact]
        public async Task AddImage_ShouldReturnIdNotExists_WhenNaturalPersonWithGivenIdDoesNotExist()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.IdNotExists;
            const string imageFileName = "image.jpg";
            var image = new FormFile(new MemoryStream(), 0, 0, "Data", imageFileName);

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);

            //Act
            var methodResult = await _sut.AddImage(It.IsAny<int>(), image);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Null(methodResult.Data);
        }

        [Fact]
        public async Task AddImage_ShouldNotAddImage_WhenNaturalPersonWithGivenIdAlreadyHasImage()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.AlreadyHaveImage;
            var naturalPerson = PreparedNaturalPersons.GetBidzinaTabagari();
            PreparedNaturalPersons.GetBidzinaTabagari().Address = "guja";
            const string imageFileName = "image.jpg";
            var image = new FormFile(new MemoryStream(), 0, 0, "Data", imageFileName);

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(naturalPerson);

            _npRepository.Setup(np =>
                np.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync(imageFileName);

            //Act
            var methodResult = await _sut.AddImage(It.IsAny<int>(), image);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Null(methodResult.Data);
        }

        [Fact]
        public async Task UpdateImage_ShouldUpdateImage_WhenAllParametersAreValid()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.ImageUpdated;
            var naturalPerson = PreparedNaturalPersons.GetBidzinaTabagari();
            const string imageFileName = "image.jpg";
            var image = new FormFile(new MemoryStream(), 0, 0, "Data", imageFileName);

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(naturalPerson);

            _npRepository.Setup(np =>
                np.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync(imageFileName);

            //Act
            var methodResult = await _sut.UpdateImage(It.IsAny<int>(), image);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(imageFileName, methodResult.Data.NaturalPersons.First().ImageFileName);
        }

        [Fact]
        public async Task UpdateImage_ShouldNotUpdateImage_WhenFileFormatIsNotValid()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.UnsupportedFileFormat;
            var naturalPerson = PreparedNaturalPersons.GetBidzinaTabagari();
            naturalPerson.ImageFileName = null;
            const string imageFileName = "image.txt";
            var image = new FormFile(new MemoryStream(), 0, 0, "Data", imageFileName);

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(naturalPerson);

            _npRepository.Setup(np =>
                np.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync(imageFileName);

            //Act
            var methodResult = await _sut.UpdateImage(It.IsAny<int>(), image);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Null(methodResult.Data);
        }

        [Fact]
        public async Task UpdateImage_ShouldReturnIdNotExists_WhenNaturalPersonWithGivenIdDoesNotExist()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.IdNotExists;
            const string imageFileName = "image.jpg";
            var image = new FormFile(new MemoryStream(), 0, 0, "Data", imageFileName);

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);

            //Act
            var methodResult = await _sut.UpdateImage(It.IsAny<int>(), image);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Null(methodResult.Data);
        }

        [Fact]
        public async Task UpdateImage_ShouldNotUpdateImage_WhenNaturalPersonWithGivenIdDoesNotHaveImage()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.NoImage;
            var naturalPerson = PreparedNaturalPersons.GetBidzinaTabagari();
            naturalPerson.ImageFileName = null;
            const string imageFileName = "image.jpg";
            var image = new FormFile(new MemoryStream(), 0, 0, "Data", imageFileName);

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(naturalPerson);

            _npRepository.Setup(np =>
                np.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync(imageFileName);

            //Act
            var methodResult = await _sut.UpdateImage(It.IsAny<int>(), image);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Null(methodResult.Data);
        }
    }
}
