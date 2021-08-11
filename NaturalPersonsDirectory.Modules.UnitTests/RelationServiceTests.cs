using Moq;
using NaturalPersonsDirectory.Common;
using NaturalPersonsDirectory.DAL;
using NaturalPersonsDirectory.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NaturalPersonsDirectory.Modules.UnitTests
{
    public class RelationServiceTests
    {
        private readonly IRelationService _sut;
        private readonly Mock<INaturalPersonRepository> _npRepository = new();
        private readonly Mock<IRelationRepository> _relationRepository = new();

        public RelationServiceTests()
        {
            _sut = new RelationService(_npRepository.Object, _relationRepository.Object);
        }

        [Fact]
        public async Task Get_ShouldReturnFirstTenRelations_WhenPaginationParametersAreDefault()
        {
            //Arrange
            const int expectedResultDataSize = 10;
            const StatusCode expectedStatusCode = StatusCode.Success;
            var paginationParameters = new RelationPaginationParameters();

            var relations = new Collection<Relation>();
            for (var i = 0; i < expectedResultDataSize; i++)
            {
                relations.Add(new Relation());
            }

            _relationRepository.Setup(r => 
                r.GetAllWithPaginationAsync(
                        It.IsAny<int>(), 
                        expectedResultDataSize, 
                        It.IsAny<bool>())).
                ReturnsAsync(relations);

            //Act
            var methodResult = await _sut.Get(paginationParameters);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(expectedResultDataSize, methodResult.Data.Count);
        }

        [Fact]
        public async Task Get_ShouldReturnPageSizeCountElements_WhenPaginationParametersAreNotDefault()
        {
            //Arrange
            const int expectedResultDataSize = 2;
            const StatusCode expectedStatusCode = StatusCode.Success;
            var paginationParameters = new RelationPaginationParameters()
            {
                PageSize = 2
            };

            var relations = new Collection<Relation>();
            for (var i = 0; i < expectedResultDataSize; i++)
            {
                relations.Add(new Relation());
            }

            _relationRepository.Setup(r => 
                r.GetAllWithPaginationAsync(
                        It.IsAny<int>(), 
                        expectedResultDataSize, 
                        It.IsAny<bool>())).
                ReturnsAsync(relations);

            //Act
            var methodResult = await _sut.Get(paginationParameters);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(expectedResultDataSize, methodResult.Data.Count);
        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_WhenThereIsNoItemInDatabase()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.NotFound;
            var paginationParameters = new RelationPaginationParameters();

            var relations = new Collection<Relation>();

            _relationRepository.Setup(r =>
                r.GetAllWithPaginationAsync(
                    It.IsAny<int>(), 
                    It.IsAny<int>(), 
                    It.IsAny<bool>())).
                ReturnsAsync(relations);

            //Act
            var methodResult = await _sut.Get(paginationParameters);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(0, methodResult.Data.Count);
        }

        [Fact]
        public async Task GetById_ShouldReturnRelation_WhenRelationWithGivenIdExists()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.Success;
            var relation = PreparedModels.GetRelation();

            _relationRepository.Setup(r =>
                r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(relation);

            //Act
            var methodResult = await _sut.GetById(It.IsAny<int>());

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(1, methodResult.Data.Count);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenRelationWithGivenIdDoesNotExist()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.NotFound;

            _relationRepository.Setup(relation =>
                relation.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);

            //Act
            var methodResult = await _sut.GetById(It.IsAny<int>());

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(0, methodResult.Data.Count);
        }

        [Fact]
        public async Task Create_ShouldCreateRelation_WhenAllParametersAreValid()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.Create;
            var relation = PreparedModels.GetRelation();
            var fromId = relation.FromId;
            var toId = relation.ToId;
            var request = new RelationRequest()
            {
                FromId = fromId,
                ToId = toId,
                RelationType = RelationType.Other
            };

            _relationRepository.Setup(r =>
                r.RelationWithGivenIdsExistAsync(fromId, toId)).ReturnsAsync(false);

            _npRepository.Setup(np =>
                np.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);

            //Act
            var methodResult = await _sut.Create(request);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(1, methodResult.Data.Count);
            Assert.Equal(fromId, methodResult.Data.Relations.First().FromId);
            Assert.Equal(toId, methodResult.Data.Relations.First().ToId);
        }

        [Fact]
        public async Task Create_ShouldNotCreateRelation_WhenRelationBetweenGivenIdsExists()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.RelationBetweenGivenIdsExists;
            var relation = PreparedModels.GetRelation();
            var fromId = relation.FromId;
            var toId = relation.ToId;
            var request = new RelationRequest()
            {
                FromId = fromId,
                ToId = toId,
                RelationType = RelationType.Other
            };

            _npRepository.Setup(np =>
                np.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);

            _relationRepository.Setup(r =>
                r.RelationWithGivenIdsExistAsync(fromId, toId)).ReturnsAsync(true);

            //Act
            var methodResult = await _sut.Create(request);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldNotCreateRelation_WhenNaturalPersonWithOneOfIdsNotExists()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.IncorrectIds;
            var relation = PreparedModels.GetRelation();
            var fromId = relation.FromId;
            var toId = relation.ToId;
            var request = new RelationRequest()
            {
                FromId = fromId,
                ToId = toId,
                RelationType = RelationType.Other
            };

            _relationRepository.Setup(r =>
                r.RelationWithGivenIdsExistAsync(fromId, toId)).ReturnsAsync(false);

            _npRepository.Setup(np =>
                np.ExistsAsync(fromId)).ReturnsAsync(false);

            _npRepository.Setup(np =>
                np.ExistsAsync(toId)).ReturnsAsync(true);

            //Act
            var methodResult = await _sut.Create(request);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
        }

        [Fact]
        public async Task Update_ShouldUpdateRelation_WhenAllParametersAreValid()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.Update;
            const RelationType changedRelationType = RelationType.Acquaintance;
            var relation = PreparedModels.GetRelation();
            var fromId = relation.FromId;
            var toId = relation.ToId;
            var request = new RelationRequest()
            {
                FromId = fromId,
                ToId = toId,
                RelationType = changedRelationType
            };

            _relationRepository.Setup(r =>
                r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(relation);

            //Act
            var methodResult = await _sut.Update(It.IsAny<int>(), request);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Equal(Enum.GetName(typeof(RelationType), changedRelationType),
                methodResult.Data.Relations.First().RelationType);
        }

        [Fact]
        public async Task Update_ShouldNotUpdateRelation_WhenRelationWithGivenIdDoesNotExist()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.IdNotExists;

            _npRepository.Setup(np =>
                np.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);

            //Act
            var methodResult = await _sut.Update(It.IsAny<int>(), new RelationRequest());

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
            Assert.Null(methodResult.Data);
        }

        [Fact]
        public async Task Update_ShouldNotUpdateRelation_WhenRelationBetweenGivenIdsDoesNotExists()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.RelationNotExists;
            const RelationType changedRelationType = RelationType.Acquaintance;
            var relation = PreparedModels.GetRelation();
            var fromId = relation.FromId;
            var toId = relation.ToId;
            var request = new RelationRequest()
            {
                FromId = fromId + 1,
                ToId = toId,
                RelationType = changedRelationType
            };

            _relationRepository.Setup(r =>
                r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(relation);

            //Act
            var methodResult = await _sut.Update(It.IsAny<int>(), request);

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldDeleteRelation_WhenRelationWithGivenIdExists()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.Delete;
            var relation = PreparedModels.GetRelation();

            _relationRepository.Setup(r =>
                r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(relation);

            //Act
            var methodResult = await _sut.Delete(It.IsAny<int>());

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldNotDeleteRelation_WhenRelationWithGivenIdDoesNotExist()
        {
            //Arrange
            const StatusCode expectedStatusCode = StatusCode.IdNotExists;

            _relationRepository.Setup(r =>
                r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);

            //Act
            var methodResult = await _sut.Delete(It.IsAny<int>());

            //Assert
            Assert.Equal(expectedStatusCode, methodResult.StatusCode);
        }
    }
}