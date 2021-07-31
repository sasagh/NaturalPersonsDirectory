using NaturalPersonsDirectory.Common;
using NaturalPersonsDirectory.DAL;
using NaturalPersonsDirectory.Models;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalPersonsDirectory.Modules
{
    public class RelationService : IRelationService
    {
        private readonly INaturalPersonRepository _npRepository;
        private readonly IRelationRepository _relationRepository;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public RelationService(INaturalPersonRepository npRepository, IRelationRepository relationRepository)
        {
            _npRepository = npRepository;
            _relationRepository = relationRepository;
        }
        public async Task<Response<RelationResponse>> Create(RelationRequest request)
        {
            var firstPersonExists = await _npRepository.ExistsAsync(request.ToId);
            var secondPersonExists = await _npRepository.ExistsAsync(request.FromId);

            var bothPersonExist = firstPersonExists && secondPersonExists;
            if (!bothPersonExist)
            {
                return ResponseHelper<RelationResponse>.GetResponse(StatusCode.IncorrectIds);
            }
            
            var relationWithGivenIdsExists =
                await _relationRepository.RelationWithGivenIdsExistAsync(request.FromId, request.ToId);

            if (relationWithGivenIdsExists)
            {
                return ResponseHelper<RelationResponse>.GetResponse(StatusCode.RelationBetweenGivenIdsExists);
            }

            var relation = new Relation()
            {
                FromId = request.FromId,
                ToId = request.ToId,
                RelationType = Enum.GetName(typeof(RelationType), request.RelationType.GetValueOrDefault())
            };

            await _relationRepository.CreateAsync(relation);

            var response = new RelationResponse(relation);

            return ResponseHelper<RelationResponse>.GetResponse(StatusCode.Create, response);
        }

        public async Task<Response<RelationResponse>> Delete(int id)
        {
            var relation = await _relationRepository.GetByIdAsync(id);

            var relationExists = relation != null;
            if (!relationExists)
            {
                return ResponseHelper<RelationResponse>.GetResponse(StatusCode.IdNotExists);
            }

            await _relationRepository.DeleteAsync(relation);

            return ResponseHelper<RelationResponse>.GetResponse(StatusCode.Delete, new RelationResponse());
        }

        public async Task<Response<RelationResponse>> Get(PaginationParameters parameters)
        {
            var relations = await 
                _relationRepository.GetAllWithPaginationAsync(
                (parameters.PageNumber - 1) * parameters.PageSize,
                parameters.PageSize);

            var atLeastOneRelationExists = relations.Any();
            if (!atLeastOneRelationExists)
            {
                return ResponseHelper<RelationResponse>.GetResponse(StatusCode.NotFound, new RelationResponse());
            }

            var response = new RelationResponse(relations);

            return ResponseHelper<RelationResponse>.GetResponse(StatusCode.Success, response);
        }

        public async Task<Response<RelationResponse>> GetById(int id)
        {
            var relation = await _relationRepository.GetByIdAsync(id);

            if (!RelationExists(relation))
            {
                return ResponseHelper<RelationResponse>.GetResponse(StatusCode.NotFound, new RelationResponse());
            }

            var response = new RelationResponse(relation);

            return ResponseHelper<RelationResponse>.GetResponse(StatusCode.Success, response);
        }

        public async Task<Response<RelationResponse>> Update(int id, RelationRequest request)
        {
            var relation = await _relationRepository.GetByIdAsync(id);

            if (!RelationExists(relation))
            {
                return ResponseHelper<RelationResponse>.GetResponse(StatusCode.IdNotExists);
            }

            var givenRelationExists = relation.FromId == request.FromId && relation.ToId == request.ToId;

            if (!givenRelationExists)
            {
                return ResponseHelper<RelationResponse>.GetResponse(StatusCode.RelationNotExists);
            }

            relation.RelationType = Enum.GetName(typeof(RelationType), request.RelationType.GetValueOrDefault());

            await _relationRepository.UpdateAsync(relation);

            var response = new RelationResponse(relation);

            return ResponseHelper<RelationResponse>.GetResponse(StatusCode.Update, response);
        }

        private static bool RelationExists(Relation relation)
        {
            return relation != null;
        }
    }
}
