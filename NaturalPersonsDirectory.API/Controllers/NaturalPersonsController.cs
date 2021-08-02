using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NaturalPersonsDirectory.Modules;
using System.Threading.Tasks;

namespace NaturalPersonsDirectory.API.Controllers
{
    [ApiController]
    [Route("api/NaturalPersons")]
    public class NaturalPersonsController : Controller
    {
        private readonly INaturalPersonService _naturalPersonService;
        public NaturalPersonsController(INaturalPersonService naturalPersonService)
        {
            _naturalPersonService = naturalPersonService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NaturalPersonResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NaturalPersonResponse>> GetNaturalPersons([FromQuery] NaturalPersonPaginationParameters parameters)
        {
            var response = await _naturalPersonService.Get(parameters);

            return response.MatchActionResult();
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NaturalPersonResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NaturalPersonResponse>> GetNaturalPerson(int id)
        {
            var response = await _naturalPersonService.GetById(id);

            return response.MatchActionResult();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NaturalPersonResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NaturalPersonResponse>> PutNaturalPerson(int id, [FromBody] NaturalPersonRequest request)
        {
            var response = await _naturalPersonService.Update(id, request);

            return response.MatchActionResult();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(NaturalPersonResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NaturalPersonResponse>> PostNaturalPerson([FromBody] NaturalPersonRequest request)
        {
            var response = await _naturalPersonService.Create(request);

            return response.MatchActionResult();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NaturalPersonResponse>> DeleteNaturalPerson(int id)
        {
            var response = await _naturalPersonService.Delete(id);

            return response.MatchActionResult();
        }

        [HttpGet("{id:int}/relations")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RelatedPersonsResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RelatedPersonsResponse>> GetRelatedPersons(int id)
        {
            var response = await _naturalPersonService.GetRelatedPersons(id);

            return response.MatchActionResult();
        }

        [HttpPost("{id:int}/image")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NaturalPersonResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NaturalPersonResponse>> AddImage(int id, IFormFile image)
        {
            var result = await _naturalPersonService.AddImage(id, image);

            return result.MatchActionResult();
        }

        [HttpPut("{id:int}/image")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NaturalPersonResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NaturalPersonResponse>> UpdateImage(int id, IFormFile image)
        {
            var result = await _naturalPersonService.UpdateImage(id, image);

            return result.MatchActionResult();
        }

        [HttpDelete("{id:int}/image")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NaturalPersonResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NaturalPersonResponse>> DeleteImage(int id)
        {
            var result = await _naturalPersonService.DeleteImage(id);

            return result.MatchActionResult();
        }
    }
}
