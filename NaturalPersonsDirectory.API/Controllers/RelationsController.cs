using Microsoft.AspNetCore.Mvc;
using NaturalPersonsDirectory.Models;
using NaturalPersonsDirectory.Modules;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NaturalPersonsDirectory.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelationsController : ControllerBase
    {
        private readonly IRelationService _relationService;

        public RelationsController(IRelationService relationService)
        {
            _relationService = relationService;
        }

        [HttpGet]
        public async Task<ActionResult<RelationResponse>> GetRelations([FromQuery] PaginationParameters parameters)
        {
            var response = await _relationService.GetAll(parameters);

            return response.MatchActionResult();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RelationResponse>> GetRelation(int id)
        {
            var response = await _relationService.GetById(id);

            return response.MatchActionResult();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<RelationResponse>> PutRelation(int id,[FromBody] RelationRequest request)
        {
            var response = await _relationService.Update(id, request);

            return response.MatchActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<RelationResponse>> PostRelation([FromBody] RelationRequest request)
        {
            var response = await _relationService.Create(request);

            return response.MatchActionResult();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<RelationResponse>> DeleteRelation(int id)
        {
            var response = await _relationService.Delete(id);

            return response.MatchActionResult();
        }
    }
}
