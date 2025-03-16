using Microsoft.EntityFrameworkCore;
using SharedExperinces.WebApi.DataAccess;
using SharedExperinces.WebApi.Models;


namespace SharedExperinces.WebApi.Services
{
    public class ServiceService
    {
        private readonly SharedExperinceContext _context;

        public ServiceService(SharedExperinceContext context)
        {
            _context = context;
        }

        public async Task<string?> AddNewService(Service service)
        {
            // Check if Provider exists
            var provider = await _context.Providers.FindAsync(service.CVR);
            if (provider == null)
                return $"Provider with CVR {service.CVR} not found.";

            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return null;
        }

        public async Task<Service?> UpdateService(Service service)
        {
            var existingServiceId = await _context.Services.FindAsync(service.ServiceId);
            if (existingServiceId == null) return null;

           _context.Services.Update(service);
            await _context.SaveChangesAsync();
            return existingServiceId;
        }

        public async Task<List<Service>?> GetAllServices()
        {
            return await _context.Services.ToListAsync();
        }

        public async Task<Service?> GetServiceById(int id)
        {
            return await _context.Services.FindAsync(id);
        }
        
        public async Task<bool> DeleteServiceById(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return false;

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
