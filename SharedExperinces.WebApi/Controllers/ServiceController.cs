using Microsoft.AspNetCore.Mvc;
using SharedExperinces.WebApi.Services;
using SharedExperinces.WebApi.DTO;
using Microsoft.Extensions.Logging;             

namespace SharedExperinces.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ServiceController : ControllerBase
	{
        private readonly ServiceService _service;
        private readonly ILogger<ServiceController> _logger;    

        public ServiceController(ServiceService service, ILogger<ServiceController> logger)                  
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
		public async Task<IActionResult> AddService([FromBody] CreateAndUpdateServiceDto service)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

            _logger.LogInformation("POST AddService {@log}",
               new { PostedBy = User.Identity?.Name, Payload = service});   

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


            _logger.LogInformation("PUT UpdateService {@log}",
                new { Id = id, PostedBy = User.Identity?.Name, Payload = service });   // NEW

            await _service.UpdateService(id, service); 

			return Ok(); 
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteService(int id)
		{
            _logger.LogInformation("DELETE DeleteService {@log}",
				 new { Id = id, PostedBy = User.Identity?.Name });         

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
