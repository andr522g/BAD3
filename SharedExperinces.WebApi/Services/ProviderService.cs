using Microsoft.EntityFrameworkCore;
using SharedExperinces.WebApi.DataAccess;
using SharedExperinces.WebApi.DTO;
using SharedExperinces.WebApi.Models;


namespace SharedExperinces.WebApi.Services
{
    public class ProviderService
    {
        private readonly SharedExperinceContext _context;

        public ProviderService(SharedExperinceContext context)
        {
            _context = context;
        }

        public async Task<List<Provider>?> GetAllProviders()
        {
            return await _context.Providers.ToListAsync();
        }

        public async Task<List<ProviderDataDto>> CollectProviderData() // Query 1
        {
            return await _context.Providers
                .Select(p => new ProviderDataDto
                {
                    Name = p.Name,
                    PhoneNumber = p.PhoneNumber
                })
                .ToListAsync();
        }

        public async Task<Provider?> GetProviderById(string id)
        {
            return await _context.Providers.FindAsync(id);
        }
    }
}
