using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedExperinces.WebApi.DataAccess;

namespace SharedExperinces.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ExperiencesController : ControllerBase
	{

		private readonly SharedExperinceContext _context;

		public ExperiencesController(SharedExperinceContext context)
		{
			_context = context;
		}


		[HttpPost]
		public async Task<IActionResult> AddExperience([FromBody] Experience experience)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);  // Returns validation error details
			}

			_context.Experiences.Add(experience);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetExperience), new { id = experience.Id }, experience);
		}

		// 2. Update the price of an experience
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateExperiencePrice(int id, [FromBody] decimal newPrice)
		{
			if (newPrice < 0)
			{
				return BadRequest("Price must be a non-negative value.");
			}

			var experience = await _context.Experiences.FindAsync(id);
			if (experience == null)
			{
				return NotFound();
			}

			experience.Price = newPrice;
			await _context.SaveChangesAsync();

			return Ok(experience);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteExperience(int id)
		{
			var experience = await _context.Experiences.FindAsync(id);
			if (experience == null)
			{
				return NotFound();
			}

			_context.Experiences.Remove(experience);
			await _context.SaveChangesAsync();

			return NoContent();  // Returns 204 No Content
		}


		// 4. Get an experience (for testing)
		[HttpGet("{id}")]
		public async Task<ActionResult<Experience>> GetExperience(int id)
		{
			var experience = await _context.Experiences.FindAsync(id);
			if (experience == null)
			{
				return NotFound();
			}

			return experience;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Experience>>> GetExperiences()
		{
			return await _context.Experiences.ToListAsync();
		}


	}
}
