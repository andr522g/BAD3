using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedExperinces.WebApi.DataAccess;
using SharedExperinces.WebApi.Models;
using System.Linq;
using System.Threading.Tasks;
using SharedExperinces.WebApi.Services;
using SharedExperinces.WebApi.DTO;

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
		public async Task<IActionResult> AddService([FromBody] CreateAndUpdateServiceDto service)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var errorMessage = await _service.AddNewService(service);

			if (errorMessage != null)
				return BadRequest(errorMessage);

			return Ok();
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateService(int id, [FromBody] CreateAndUpdateServiceDto service)
		{
			if (!ModelState.IsValid) // This checks the custom validation on Price
			{
				return BadRequest(ModelState); // If validation fails, return BadRequest
			}

			await _service.UpdateService(id, service); 

			return Ok(); 
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteService(int id)
		{
			var service = await _service.DeleteServiceById(id);

			if (service == false)
			{
				return NotFound();
			}

			return Ok();
		}

		[HttpGet("GetAllServicesBasic")] // Query 2
		public async Task<IActionResult> GetAllServicesBasic()
		{
            var services = await _service.GetAllServicesBasic();
            if (services == null || !services.Any())
                return NotFound("No services found");
            return Ok(services);
        }

		[HttpGet("GetGroupsizes")]
        public async Task<IActionResult> GetAllGroupSizes()
		{
            var groupSizes = await _service.GetAllGroupSizes();
            if (groupSizes == null || !groupSizes.Any())
                return NotFound("No group sizes found");
            return Ok(groupSizes);
        }

    }
}
