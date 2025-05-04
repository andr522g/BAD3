using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SharedExperinces.WebApi.Services;

namespace SharedExperinces.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly ProviderService _service;

        public ProviderController(ProviderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProviders()
        {
            var service = await _service.GetAllProviders();
            return Ok(service);
        }

        [HttpGet("CollectProviderData")] // Query 1
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CollectProviderData()
        {
            var service = await _service.CollectProviderData();
            if (service == null || !service.Any()) return NotFound("No providers found");
            return Ok(service);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProvider(string id)
        {
            var service = await _service.GetProviderById(id);
            if (service == null) return NotFound();
            return Ok(service);
        }
    }
}
