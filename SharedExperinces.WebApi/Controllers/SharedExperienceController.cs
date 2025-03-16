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

        [HttpGet("ListExperiencesWithDates")]
		public async Task<IActionResult> GetSharedExperienceDates()
		{
			var experiences = await _service.GetAllExperienceDate();
			if (experiences == null || !experiences.Any()) // Checks if the data exists nad if the list is empty
				return NotFound("No shared expereinces found");

			return Ok(experiences);
		}

		[HttpGet("GetGuestsRegisteredForExperience")]
		public async Task<IActionResult> GetAllGuestsInSharedExperience(int id)
		{
			var guests = await _service.GetAllGuestsInSharedExperience(id);
			if (guests == null || !guests.Any()) return NotFound("No guests found in shared experience");

			return Ok(guests);
		}

		[HttpGet("GetServicesInSharedExperience")]
		public async Task<IActionResult> GetAllServicesInSharedExperience(int id)
		{
			var services = await _service.GetAllServicesInSharedExperience(id);
			if (services == null || !services.Any()) return NotFound("No guests found in shared experience");

			return Ok(services);
		}

		[HttpGet("GetGuestsRegisteredForServiceInSharedExperience")]
		public async Task<IActionResult> GetGuestsInServiceInSharedExperience(int SeId, int sId)
		{
			var guests = await _service.GetGuestsInServiceInSharedExperience(SeId, sId);
			if (guests == null || !guests.Any())
				return NotFound($"No guests found in service in this shared expereince");

			return Ok(guests);
        }
    }
}
