using SharedExperinces.WebApi.DataAccess;
using SharedExperinces.WebApi.Models;
using SharedExperinces.WebApi.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;


namespace SharedExperinces.WebApi.Services
{
	public class SharedExperienceService
	{
		private readonly SharedExperinceContext _context;

		public SharedExperienceService(SharedExperinceContext context)
		{
			_context = context;
		}

		public async Task<string?> createExperince(SharedExperience experience) 
		{
            _context.SharedExperiences.Add(experience);
			await _context.SaveChangesAsync();

			return null;
		}    

        public async Task<List<SharedExperienceDateDto>> GetAllExperienceDate()
        {
            return await _context.SharedExperiences
                .Select(se => new SharedExperienceDateDto
                {
                    SharedExperienceId = se.SharedExperienceId,
                    Name = se.Name,
                    EarliestServiceDate = se.Services.Any()
                        ? se.Services.Min(s => s.ServiceDate)  // Get the earliest date
                        : (DateTime?)null  // If no services exist, return null
                })
                .OrderByDescending(se => se.EarliestServiceDate) // Sort in descending order
                .ToListAsync();
        }

        public async Task<List<Guest>?> GetAllGuestsInSharedExperience(int id)
        {
            return await _context.SharedExperiences
                .Where(se => se.SharedExperienceId == id)
                .SelectMany(se => se.Guests)
                .ToListAsync();
        }

        public async Task<List<Service>?> GetAllServicesInSharedExperience(int id)
        {
            return await _context.SharedExperiences
                .Where(se => se.SharedExperienceId == id)
                .SelectMany(se => se.Services)
                .ToListAsync();
        }
    }
}
