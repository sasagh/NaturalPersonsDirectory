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
        public async Task<ActionResult<NaturalPersonResponse>> GetNaturalPersons([FromQuery] PaginationParameters parameters)
        {
            var response = await _naturalPersonService.GetAll(parameters);

            return response.MatchActionResult();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<NaturalPersonResponse>> GetNaturalPerson(int id)
        {
            var response = await _naturalPersonService.GetById(id);

            return response.MatchActionResult();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<NaturalPersonResponse>> PutNaturalPerson(int id, [FromBody] NaturalPersonRequest request)
        {
            var response = await _naturalPersonService.Update(id, request);

            return response.MatchActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<NaturalPersonResponse>> PostNaturalPerson([FromBody] NaturalPersonRequest request)
        {
            var response = await _naturalPersonService.Create(request);

            return response.MatchActionResult();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<NaturalPersonResponse>> DeleteNaturalPerson(int id)
        {
            var response = await _naturalPersonService.Delete(id);

            return response.MatchActionResult();
        }

        [HttpGet("{id:int}/relations")]
        public async Task<ActionResult<RelatedPersonsResponse>> GetRelatedPersons(int id)
        {
            var response = await _naturalPersonService.GetRelatedPersons(id);

            return response.MatchActionResult();
        }

        [HttpPost("{id:int}/image")]
        public async Task<ActionResult<NaturalPersonResponse>> AddImage(int id, IFormFile image)
        {
            var result = await _naturalPersonService.AddImage(id, image);

            return result.MatchActionResult();
        }

        [HttpPut("{id:int}/image")]
        public async Task<ActionResult<NaturalPersonResponse>> UpdateImage(int id, IFormFile image)
        {
            var result = await _naturalPersonService.UpdateImage(id, image);

            return result.MatchActionResult();
        }

        [HttpDelete("{id:int}/image")]
        public async Task<ActionResult<NaturalPersonResponse>> DeleteImage(int id)
        {
            var result = await _naturalPersonService.DeleteImage(id);

            return result.MatchActionResult();
        }
    }
}
