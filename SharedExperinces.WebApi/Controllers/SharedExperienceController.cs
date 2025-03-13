using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedExperinces.WebApi.DataAccess;
using SharedExperinces.WebApi.Models;

namespace SharedExperinces.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SharedExperienceController : ControllerBase
	{

		private readonly SharedExperinceContext _context;

		public SharedExperienceController(SharedExperinceContext context)
		{
			_context = context;
		}


		[HttpPost]
		public async Task<IActionResult> AddExperience([FromBody] SharedExperience experience)
		{

			if (experience == null) 
			{
				return BadRequest("Experience data is required.");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState); 
			}

			_context.SharedExperiences.Add(experience);
			await _context.SaveChangesAsync();
			return Ok(experience);
		}


	}
}
