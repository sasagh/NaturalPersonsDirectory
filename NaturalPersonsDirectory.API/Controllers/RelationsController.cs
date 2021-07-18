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
        public async Task<ActionResult<IEnumerable<Relation>>> GetRelations([FromQuery] PaginationParameters parameters)
        {
            var response = await _relationService.Get(parameters);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Relation>> GetRelation(int id)
        {
            var response = await _relationService.GetById(id);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRelation(int id,[FromBody] RelationRequest request)
        {
            var response = await _relationService.Update(id, request);

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<Relation>> PostRelation([FromBody] RelationRequest request)
        {
            var response = await _relationService.Create(request);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Relation>> DeleteRelation(int id)
        {
            var response = await _relationService.Delete(id);

            return Ok(response);
        }
    }
}
