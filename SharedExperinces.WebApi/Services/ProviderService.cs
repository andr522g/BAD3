using Microsoft.EntityFrameworkCore;
using SharedExperinces.WebApi.DataAccess;
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

        public async Task<Provider?> GetProviderById(string id)
        {
            return await _context.Providers.FindAsync(id);
        }
    }
}
