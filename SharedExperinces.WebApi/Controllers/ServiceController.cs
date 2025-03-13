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

		
	}
}
