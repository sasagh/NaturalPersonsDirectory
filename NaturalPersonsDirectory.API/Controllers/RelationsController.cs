using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NaturalPersonsDirectory.Modules;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RelationResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RelationResponse>> GetRelations([FromQuery] PaginationParameters parameters)
        {
            var response = await _relationService.Get(parameters);

            return response.MatchActionResult();
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RelationResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RelationResponse>> GetRelation(int id)
        {
            var response = await _relationService.GetById(id);

            return response.MatchActionResult();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RelationResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RelationResponse>> PutRelation(int id, [FromBody] RelationRequest request)
        {
            var response = await _relationService.Update(id, request);

            return response.MatchActionResult();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RelationResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RelationResponse>> PostRelation([FromBody] RelationRequest request)
        {
            var response = await _relationService.Create(request);

            return response.MatchActionResult();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RelationResponse>> DeleteRelation(int id)
        {
            var response = await _relationService.Delete(id);

            return response.MatchActionResult();
        }
    }
}
