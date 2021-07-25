using Microsoft.EntityFrameworkCore;
using NaturalPersonsDirectory.Common;
using NaturalPersonsDirectory.Db;
using NaturalPersonsDirectory.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace NaturalPersonsDirectory.Modules
{
    public class RelationService : IRelationService
    {
        private readonly ApplicationDbContext _context;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public RelationService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Response<RelationResponse>> Create(RelationRequest request)
        {
            var relationWithGivenIds = await _context
                .Relations
                .SingleOrDefaultAsync(relation => 
                    relation.FromId == request.FromId && relation.ToId == request.ToId 
                    || relation.ToId == request.FromId && relation.FromId == request.ToId);

            var relationWithGivenIdsExists = relationWithGivenIds != null;

            if (relationWithGivenIdsExists)
            {
                return ResponseHelper<RelationResponse>.GetResponse(StatusCode.RelationBetweenGivenIdsExists);
            }

            var relationFrom =
                await _context
                    .NaturalPersons
                    .FirstOrDefaultAsync(naturalPerson => naturalPerson.Id == request.FromId);
            var relationTo =
                await _context
                    .NaturalPersons
                    .FirstOrDefaultAsync(naturalPerson => naturalPerson.Id == request.ToId);

            var bothPersonExist = relationFrom != null && relationTo != null;

            if (!bothPersonExist)
            {
                return ResponseHelper<RelationResponse>.GetResponse(StatusCode.IncorrectIds);
            }

            var relation = new Relation()
            {
                FromId = request.FromId,
                ToId = request.ToId,
                RelationType = request.RelationType.GetValueOrDefault()
            };

            _context.Relations.Add(relation);
            await _context.SaveChangesAsync();

            var response = new RelationResponse()
            {
                Relations = new List<Relation>() { relation }
            };

            return ResponseHelper<RelationResponse>.GetResponse(StatusCode.Success, response);
        }

        public async Task<Response<RelationResponse>> Delete(int id)
        {
            var relation = await _context.Relations.SingleOrDefaultAsync(relation => relation.Id == id);

            if (relation == null)
            {
                return ResponseHelper<RelationResponse>.GetResponse(StatusCode.IdNotExists);
            }

            _context.Relations.Remove(relation);
            await _context.SaveChangesAsync();

            return ResponseHelper<RelationResponse>.GetResponse(StatusCode.Delete, new RelationResponse());
        }

        public async Task<Response<RelationResponse>> GetAll(PaginationParameters parameters)
        {
            var relations = await _context
                .Relations
                .Include(relation => relation.From)
                .Include(relation => relation.To)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            if (!relations.Any())
            {
                return ResponseHelper<RelationResponse>.GetResponse(StatusCode.NotFound);
            }

            var response = new RelationResponse()
            {
                Relations = relations
            };

            return ResponseHelper<RelationResponse>.GetResponse(StatusCode.Success, response);
        }

        public async Task<Response<RelationResponse>> GetById(int id)
        {
            var relation = await _context
                .Relations
                .Include(relation => relation.From)
                .Include(relation => relation.To)
                .FirstOrDefaultAsync(relation => relation.Id == id);

            if (relation == null)
            {
                return ResponseHelper<RelationResponse>.GetResponse(StatusCode.NotFound);
            }
            
            var response = new RelationResponse()
            {
                Relations = new List<Relation>() { relation }
            };

            return ResponseHelper<RelationResponse>.GetResponse(StatusCode.Success, response);
        }

        public async Task<Response<RelationResponse>> Update(int id, RelationRequest request)
        {
            var relation = await _context
                .Relations
                .Include(relation => relation.From)
                .Include(relation => relation.To)
                .SingleOrDefaultAsync(relation => relation.Id == id);

            if (relation == null)
            {
                return ResponseHelper<RelationResponse>.GetResponse(StatusCode.IdNotExists);
            }

            var relationBetweenGivenIdsExists = relation.FromId == request.FromId && relation.ToId == request.ToId;
            
            if(!relationBetweenGivenIdsExists)
            {
                return ResponseHelper<RelationResponse>.GetResponse(StatusCode.RelationNotExists);
            }

            relation.RelationType = request.RelationType.GetValueOrDefault();

            _context.Update(relation);
            await _context.SaveChangesAsync();

            var response = new RelationResponse()
            {
                Relations = new List<Relation>() { relation }
            };

            return ResponseHelper<RelationResponse>.GetResponse(StatusCode.Update, response);
        }
    }
}
