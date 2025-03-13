using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedExperinces.WebApi.DataAccess;
using SharedExperinces.WebApi.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SharedExperinces.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ServiceController : ControllerBase
	{
		private readonly SharedExperinceContext _context;

		public ServiceController(SharedExperinceContext context)
		{
			_context = context;
		}

		[HttpPost]
		public async Task<IActionResult> AddService([FromBody] Service service)
		{
			if (service == null)
				return BadRequest("Service data is required.");

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			// Check if Provider exists
			var provider = await _context.Providers.FindAsync(service.CVR);
			if (provider == null)
				return NotFound($"Provider with CVR {service.CVR} not found.");

			_context.Services.Add(service);
			await _context.SaveChangesAsync();

			return Ok();
		}


		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateService(int id, [FromBody] Service service)
		{
			if (!ModelState.IsValid) // This checks the custom validation on Price
			{
				return BadRequest(ModelState); // If validation fails, return BadRequest
			}

			var existingService = await _context.Services.FindAsync(id);
			if (existingService == null)
			{
				return NotFound(); // If the service with the given id is not found
			}

		
			existingService.Price = service.Price; // This will trigger the Price validation
			

			_context.Services.Update(existingService);
			await _context.SaveChangesAsync();

			return Ok(existingService); // Return the updated service
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteService(int id)
		{
			var service = await _context.Services.FindAsync(id);

			if (service == null)
			{
				return NotFound();
			}

			_context.Services.Remove(service);

			await _context.SaveChangesAsync();

			return NoContent();
		}




	}
}
