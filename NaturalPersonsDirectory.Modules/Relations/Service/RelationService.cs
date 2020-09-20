using Microsoft.EntityFrameworkCore;
using NaturalPersonsDirectory.Common;
using NaturalPersonsDirectory.Db;
using NaturalPersonsDirectory.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NaturalPersonsDirectory.Modules
{
    public class RelationService : IRelationService
    {
        private readonly NaturalPersonsDirectoryDbContext _context;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public RelationService(NaturalPersonsDirectoryDbContext context)
        {
            _context = context;
        }
        public async Task<IResponse<RelationResponse>> Create(RelationRequest request)
        {
            var relationFrom = await _context.NaturalPersons.FirstOrDefaultAsync(naturalPerson => naturalPerson.NaturalPersonId == request.FromId);
            var relationTo = await _context.NaturalPersons.FirstOrDefaultAsync(naturalPerson => naturalPerson.NaturalPersonId == request.ToId);
            var relationWithSameIds = await _context.Relations.SingleOrDefaultAsync(relation => relation.FromId == request.FromId && relation.ToId == request.ToId);
            var relationWithReversedIds = await _context.Relations.SingleOrDefaultAsync(relation => relation.FromId == request.ToId && relation.ToId == request.FromId);

            try
            {
                if (!(relationFrom != null && relationTo != null && request.FromId != request.ToId && relationWithSameIds == null && relationWithReversedIds == null))
                {
                    return ResponseHelper.Fail<RelationResponse>(StatusCode.IncorrectIds);
                }

                _context.RelationIds.Add(new RelationId());
                await _context.SaveChangesAsync();

                var relationId = await _context.RelationIds.OrderByDescending(id => id.Id).FirstOrDefaultAsync();
                var relation = new Relation()
                {
                    RelationId = relationId.Id,
                    FromId = request.FromId,
                    ToId = request.ToId,
                    RelationType = request.RelationType
                };

                _context.Relations.Add(relation);
                await _context.SaveChangesAsync();

                var response = new RelationResponse()
                {
                    Relations = new List<Relation>() { relation }
                };

                return ResponseHelper.Ok(response);
            }
            catch(Exception ex)
            {
                return CatchException(ex);
            }
        }

        public async Task<IResponse<RelationResponse>> Delete(int id)
        {
            try
            {
                var relation = await _context.Relations.SingleOrDefaultAsync(relation => relation.RelationId == id);

                if (relation == null)
                {
                    return ResponseHelper.Fail<RelationResponse>(StatusCode.IdNotExists);
                }

                _context.Relations.Remove(relation);
                await _context.SaveChangesAsync();

                var response = new RelationResponse { Relations = new List<Relation>() };
                return ResponseHelper.Ok(response, StatusCode.Delete);
            }
            catch(Exception ex)
            {
                return CatchException(ex);
            }
        }

        public async Task<IResponse<RelationResponse>> Get(PaginationParameters parameters)
        {
            try
            {
                var relations = await _context
                .Relations
                .Include(relation => relation.RelationFrom)
                .Include(relation => relation.RelationTo)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

                var response = new RelationResponse()
                {
                    Relations = relations.Any() ? relations : new List<Relation>()
                };

                return relations.Any() ? ResponseHelper.Ok(response) : ResponseHelper.NotFound(response);
            }
            catch(Exception ex)
            {
                return CatchException(ex);
            }
        }

        public async Task<IResponse<RelationResponse>> GetById(int id)
        {
            try
            {
                var relation = await _context
                .Relations
                .Include(relation => relation.RelationFrom)
                .Include(relation => relation.RelationTo)
                .FirstOrDefaultAsync(relation => relation.RelationId == id);

                var response = new RelationResponse()
                {
                    Relations = relation != null ? new List<Relation>() { relation } : new List<Relation>()
                };

                return relation != null ? ResponseHelper.Ok(response) : ResponseHelper.NotFound(response);
            }
            catch(Exception ex)
            {
                return CatchException(ex);
            }
        }

        public async Task<IResponse<RelationResponse>> Update(int id, RelationRequest request)
        {
            try
            {
                var relation = await _context
                    .Relations
                    .Include(relation => relation.RelationFrom)
                    .Include(relation => relation.RelationTo)
                    .SingleOrDefaultAsync(relation => relation.RelationId == id);

                if (relation == null)
                {
                    return ResponseHelper.Fail<RelationResponse>(StatusCode.IdNotExists);
                }

                if(!(relation.FromId == request.FromId && relation.ToId == request.ToId))
                {
                    return ResponseHelper.Fail<RelationResponse>();
                }

                relation.RelationType = request.RelationType;

                _context.Update(relation);
                await _context.SaveChangesAsync();

                var response = new RelationResponse()
                {
                    Relations = new List<Relation>() { relation }
                };

                return ResponseHelper.Ok(response, StatusCode.Update);
            }
            catch(Exception ex)
            {
                return CatchException(ex);
            }
        }

        private IResponse<RelationResponse> CatchException(Exception ex)
        {
            _logger.Error(ex.Message);
            return ResponseHelper.Fail<RelationResponse>();
        }
    }
}
