using SharedExperinces.WebApi.DataAccess;
using SharedExperinces.WebApi.Models;

namespace SharedExperinces.WebApi.Services
{
	public class SharedExperienceService
	{
		private readonly SharedExperinceContext _context;

		public SharedExperienceService(SharedExperinceContext context)
		{
			_context = context;
		}

		public async Task createExperince(SharedExperience experience) 
		{

			_context.SharedExperiences.Add(experience);
			await _context.SaveChangesAsync();
		}
	}
}
