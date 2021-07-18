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
        public async Task<IActionResult> GetNaturalPersons([FromQuery] PaginationParameters parameters)
        {
            var response = await _naturalPersonService.Get(parameters);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNaturalPerson(int id)
        {
            var response = await _naturalPersonService.GetById(id);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutNaturalPerson(int id, [FromBody] NaturalPersonRequest request)
        {
            var response = await _naturalPersonService.Update(id, request);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> PostNaturalPerson([FromBody] NaturalPersonRequest request)
        {
            var response = await _naturalPersonService.Create(request);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNaturalPerson(int id)
        {
            var response = await _naturalPersonService.Delete(id);

            return Ok(response);
        }

        [HttpGet("{id}/relations")]
        public async Task<IActionResult> GetRelatedPersons(int id)
        {
            var response = await _naturalPersonService.GetRelatedPersons(id);

            return Ok(response);
        }

        [HttpPost("{id}/image")]
        public async Task<IActionResult> AddImage(int id, IFormFile image)
        {
            var result = await _naturalPersonService.AddImage(id, image);

            return Ok(result);
        }

        [HttpPut("{id}/image")]
        public async Task<IActionResult> UpdateImage(int id, IFormFile image)
        {
            var result = await _naturalPersonService.UpdateImage(id, image);

            return Ok(result);
        }

        [HttpDelete("{id}/image")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var result = await _naturalPersonService.DeleteImage(id);

            return Ok(result);
        }
    }
}
