using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedExperinces.WebApi.DataAccess;
using SharedExperinces.WebApi.Models;
using System.Linq;
using System.Threading.Tasks;
using SharedExperinces.WebApi.Services;

namespace SharedExperinces.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ServiceController : ControllerBase
	{
		private readonly ServiceService _service;

		public ServiceController(ServiceService service)
		{
			_service = service;
		}

		[HttpPost]
		public async Task<IActionResult> AddService([FromBody] Service service)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var errorMessage = await _service.AddNewService(service);

			if (errorMessage != null)
				return BadRequest(errorMessage);

			return Ok();
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateService(int id, [FromBody] Service service)
		{
			if (!ModelState.IsValid) // This checks the custom validation on Price
			{
				return BadRequest(ModelState); // If validation fails, return BadRequest
			}

			var existingService = await _service.GetServiceById(id);

            if (existingService == null)
			{
				return NotFound(); // If the service with the given id is not found
			}
		
			existingService.Price = service.Price; // This will trigger the Price validation

			return Ok(existingService); // Return the updated service
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteService(int id)
		{
			var service = await _service.DeleteServiceById(id);

			if (service == false)
			{
				return NotFound();
			}

			return NoContent();
		}
	}
}
