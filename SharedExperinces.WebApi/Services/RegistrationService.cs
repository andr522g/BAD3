using Microsoft.EntityFrameworkCore;
using SharedExperinces.WebApi.DataAccess;
using SharedExperinces.WebApi.Models;


namespace SharedExperinces.WebApi.Services
{
    public class RegistrationService
    {
        private readonly SharedExperinceContext _context;

        public RegistrationService(SharedExperinceContext context)
        {
            _context = context;
        }

        public async Task<List<Guest>?> GetGuestsRegisteredInSharedExperience(int SeId, int sId)
        {
            return await _context.Registrations
                .Where(r => r.ServiceId == sId)
                .ToListAsync();
        }

    }
}
