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

        public async Task<List<SharedExperienceDateDto>> GetAllExperienceDate() // Query 3
        {
            return await _context.SharedExperiences
                .Select(se => new SharedExperienceDateDto
                {
                    Name = se.Name,
                    EarliestServiceDate = se.Services.Any()
                        ? se.Services.Min(s => s.ServiceDate)  // Get the earliest date
                        : (DateTime?)null  // If no services exist, return null
                })
                .OrderByDescending(se => se.EarliestServiceDate) // Sort in descending order
                .ToListAsync();
        }

        public async Task<List<GuestNameDto>?> GetAllGuestsInSharedExperience(int id) // Query 4
        {
            return await _context.SharedExperiences
                .Where(se => se.SharedExperienceId == id)
                .SelectMany(se => se.Services)
                .SelectMany(s => s.Registrations.Select(r => new GuestNameDto
                {
                    Name = r.Guest.Name
                }))
                .ToListAsync();
        }

        public async Task<List<ServiceNameDto>?> GetAllServicesInSharedExperience(int id) // Query 5
        {
            return await _context.SharedExperiences
                .Where(se => se.SharedExperienceId == id)
                .SelectMany(se => se.Services)
                .Select(s => new ServiceNameDto
                {
                    Name = s.Name
                })
                .ToListAsync();
        }

        public async Task<List<GuestNameDto>?> GetGuestsInServiceInSharedExperience(int seId, int sId) // Query 6
        {
            return await _context.SharedExperiences
                .Where(se => se.SharedExperienceId == seId)  // Filter by SharedExperience ID
                .SelectMany(se => se.Services
                    .Where(s => s.ServiceId == sId)  // Filter by Service ID
                    .SelectMany(s => s.Registrations.Select(r => r.Guest))
                    .Select(g => new GuestNameDto
                    {
                        Name = g.Name
                    })
                )
                .ToListAsync();
        }

        public async Task<MinAvgMaxPriceDto> MinAvgMaxPrice(int seId) // Query 7
        {
            var minPrice = await _context.SharedExperiences
                .Where(se => se.SharedExperienceId == seId)
                .SelectMany(se => se.Services)
                .Select(p => p.Price)
                .MinAsync();

            var AvgPrice = await _context.SharedExperiences
                .Where(se => se.SharedExperienceId == seId)
                .SelectMany(se => se.Services)
                .Select(p => p.Price)
                .AverageAsync();

            var MaxPrice = await _context.SharedExperiences
                .Where(se => se.SharedExperienceId == seId)
                .SelectMany(se => se.Services)
                .Select(p => p.Price)
                .MaxAsync();

            return new MinAvgMaxPriceDto()
            {
                MinPrice = minPrice,
                AvgPrice = (int)AvgPrice,
                MaxPrice = MaxPrice
            };
        }

        public async Task<List<ServiceGuestsSalesDto>?> GetServiceGuestSales() // Query 8
        {
            return await _context.SharedExperiences
                .Select(se => new ServiceGuestsSalesDto
                {
                    ServiceName = se.Name,
                    GuestCount = se.Services
                        .SelectMany(s => s.Registrations)
                        .Count(),
                    TotalSales = se.Services
                        .SelectMany(s => s.Registrations)
                        .Sum(r => r.Service.Price)
                })
                .ToListAsync();
        }
    }
}
