using Moq;
using NaturalPersonsDirectory.DAL;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NaturalPersonsDirectory.Common;
using NaturalPersonsDirectory.Models;
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
            
            _npRepository.Setup(np => np.GetAllWithPagination(It.IsAny<int>(), It.IsAny<int>()).Result).Returns(naturalPersons);
            
            //Act
            var methodResult = await _sut.GetAll(paginationParameters);
            
            //Assert
            Assert.Equal(expectedResultDataSize, methodResult.Data.Count);
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
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
                np.GetAllWithPagination(It.IsAny<int>(), expectedResultDataSize).Result).Returns(naturalPersons);
            
            //Act
            var methodResult = await _sut.GetAll(paginationParameters);
            
            //Assert
            Assert.Equal(expectedResultDataSize, methodResult.Data.Count);
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
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
            Assert.Null(methodResult.Data);
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldCreateNaturalPerson_WhenAllParametersAreCorrect()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.Create;
            var naturalPerson = PreparedNaturalPersons.BidzinaTabagari;
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
                np.CreateAsync(naturalPerson)).ReturnsAsync(naturalPerson);
            
            //Act
            var methodResult = await _sut.Create(request);
            
            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(naturalPerson.PassportNumber, methodResult.Data.NaturalPersons.First().PassportNumber);
        }
    }
}
