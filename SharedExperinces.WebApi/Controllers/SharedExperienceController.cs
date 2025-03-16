using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedExperinces.WebApi.DataAccess;
using SharedExperinces.WebApi.Models;
using SharedExperinces.WebApi.Services;

namespace SharedExperinces.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SharedExperienceController : ControllerBase
	{

		private readonly SharedExperienceService _service;

		public SharedExperienceController(SharedExperienceService service)
		{
			_service = service;
		}


		[HttpPost]
		public async Task<IActionResult> AddExperience([FromBody] SharedExperience experience)
		{
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.createExperince(experience);

			return Ok(experience);
		}





	}
}
