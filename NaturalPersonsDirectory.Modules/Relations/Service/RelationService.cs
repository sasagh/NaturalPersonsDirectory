﻿using NaturalPersonsDirectory.Common;
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
            var relationWithGivenIdsExists =
                await _relationRepository.RelationWithGivenIdsExist(request.FromId, request.ToId);

            if (relationWithGivenIdsExists)
            {
                return ResponseHelper<RelationResponse>.GetResponse(StatusCode.RelationBetweenGivenIdsExists);
            }

            var relationFrom = await _npRepository.GetByIdAsync(request.ToId);
            var relationTo = await _npRepository.GetByIdAsync(request.FromId);

            var bothPersonExist = relationFrom != null && relationTo != null;
            if (!bothPersonExist)
            {
                return ResponseHelper<RelationResponse>.GetResponse(StatusCode.IncorrectIds);
            }

            var relation = new Relation()
            {
                FromId = request.FromId,
                ToId = request.ToId,
                RelationType = Enum.GetName(typeof(RelationType), request.RelationType.GetValueOrDefault())
            };

            await _relationRepository.CreateAsync(relation);

            var response = new RelationResponse(relation);

            return ResponseHelper<RelationResponse>.GetResponse(StatusCode.Success, response);
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

        public async Task<Response<RelationResponse>> GetAll(PaginationParameters parameters)
        {
            var rel = await _relationRepository.GetAllWithPagination((parameters.PageNumber - 1) * parameters.PageSize,
                parameters.PageSize);

            var relations = rel.ToList();

            var atLeastOneRelationExists = relations.Any();
            if (!atLeastOneRelationExists)
            {
                return ResponseHelper<RelationResponse>.GetResponse(StatusCode.NotFound);
            }

            var response = new RelationResponse(relations);

            return ResponseHelper<RelationResponse>.GetResponse(StatusCode.Success, response);
        }

        public async Task<Response<RelationResponse>> GetById(int id)
        {
            var relation = await _relationRepository.GetByIdAsync(id);

            if (!RelationExists(relation))
            {
                return ResponseHelper<RelationResponse>.GetResponse(StatusCode.NotFound);
            }

            var response = new RelationResponse(relation);

            return ResponseHelper<RelationResponse>.GetResponse(StatusCode.Success, response);
        }

        public async Task<Response<RelationResponse>> Update(int id, RelationRequest request)
        {
            var relation = await _relationRepository.GetByIdAsync(id);

            if (relation == null)
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
