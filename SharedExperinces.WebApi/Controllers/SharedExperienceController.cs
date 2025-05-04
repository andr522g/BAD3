using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedExperinces.WebApi.DataAccess;
using SharedExperinces.WebApi.Models;
using SharedExperinces.WebApi.Services;
using Microsoft.Extensions.Logging;                // NEW

namespace SharedExperinces.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SharedExperienceController : ControllerBase
    {
        private readonly SharedExperienceService _service;
        private readonly ILogger<SharedExperienceController> _logger;  

        public SharedExperienceController(SharedExperienceService service, ILogger<SharedExperienceController> logger)               
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
		public async Task<IActionResult> AddExperience([FromBody] SharedExperience experience)
		{
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("POST AddExperience {@log}",
                new { PostedBy = User.Identity?.Name, Payload = experience });   

            await _service.createExperince(experience);

			return Ok(experience);
		}

        [HttpGet("ListExperiencesWithDates")]
		public async Task<IActionResult> GetSharedExperienceDates() // Query 3
        {
			var experiences = await _service.GetAllExperienceDate();
			if (experiences == null || !experiences.Any()) // Checks if the data exists nad if the list is empty
				return NotFound("No shared expereinces found");

			return Ok(experiences);
		}

		[HttpGet("GetGuestsRegisteredForExperience")] // Query 4
		public async Task<IActionResult> GetAllGuestsInSharedExperience(int id)
		{
			var guests = await _service.GetAllGuestsInSharedExperience(id);
			if (guests == null || !guests.Any()) return NotFound("No guests found in shared experience");

			return Ok(guests);
		}

		[HttpGet("GetServicesInSharedExperience")] // Query 5
        public async Task<IActionResult> GetAllServicesInSharedExperience(int id)
		{
			var services = await _service.GetAllServicesInSharedExperience(id);
			if (services == null || !services.Any()) return NotFound("No guests found in shared experience");

			return Ok(services);
		}

		[HttpGet("GetGuestsRegisteredForServiceInSharedExperience")] // Query 6
        public async Task<IActionResult> GetGuestsInServiceInSharedExperience(int SeId, int sId)
		{
			var guests = await _service.GetGuestsInServiceInSharedExperience(SeId, sId);
			if (guests == null || !guests.Any())
				return NotFound($"No guests found in service in this shared expereince");

			return Ok(guests);
        }

		[HttpGet("MinAvgMaxPrice")] // Query 7
        public async Task<IActionResult> MinAvgMaxPrice(int SeId)
		{
            var prices = await _service.MinAvgMaxPrice(SeId);
            if (prices == null)
                return NotFound("No prices found");
            return Ok(prices);
        }

		[HttpGet("GetServiceGuestSales")] // Query 8
        public async Task<IActionResult> GetServiceGuestSales()
		{
            var sales = await _service.GetServiceGuestSales();
            if (sales == null || !sales.Any())
                return NotFound("No sales found");
            return Ok(sales);
        }
    }
}
